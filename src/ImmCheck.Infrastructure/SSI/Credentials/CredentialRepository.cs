using ImmCheck.Core.SSI.Credentials;
using ImmCheck.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ImmCheck.Infrastructure.SSI.Credentials;

public class CredentialRepository : ICredentialRepository
{
    private readonly AppDbContext _db;

    public CredentialRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IssuedCredentialRecord> StoreAsync(IssuedCredentialRecord record)
    {
        _db.IssuedCredentials.Add(record);
        await _db.SaveChangesAsync();
        return record;
    }

    public async Task<IssuedCredentialRecord?> GetByIdAsync(string credentialId)
    {
        return await _db.IssuedCredentials.FindAsync(credentialId);
    }

    public async Task<IEnumerable<IssuedCredentialRecord>> GetBySubjectAsync(string subjectDid)
    {
        return await _db.IssuedCredentials
            .Where(c => c.SubjectDid == subjectDid)
            .OrderByDescending(c => c.IssuedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<IssuedCredentialRecord>> GetByIssuerAsync(string issuerDid)
    {
        return await _db.IssuedCredentials
            .Where(c => c.IssuerDid == issuerDid)
            .OrderByDescending(c => c.IssuedAt)
            .ToListAsync();
    }

    public async Task<bool> RevokeAsync(string credentialId)
    {
        var record = await _db.IssuedCredentials.FindAsync(credentialId);
        if (record == null || record.IsRevoked) return false;

        record.IsRevoked = true;
        record.RevokedAt = DateTime.UtcNow;

        // Update the status list
        var statusList = await GetOrCreateStatusListAsync(record.IssuerDid);
        SetBit(statusList, record.StatusListIndex, true);
        statusList.LastUpdated = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<StatusListRecord?> GetStatusListAsync(string issuerDid)
    {
        return await _db.StatusLists.FindAsync(issuerDid);
    }

    public async Task<StatusListRecord> UpdateStatusListAsync(StatusListRecord statusList)
    {
        _db.StatusLists.Update(statusList);
        await _db.SaveChangesAsync();
        return statusList;
    }

    public async Task<int> AllocateStatusListIndexAsync(string issuerDid)
    {
        var statusList = await GetOrCreateStatusListAsync(issuerDid);
        var index = statusList.NextIndex;
        statusList.NextIndex++;
        await _db.SaveChangesAsync();
        return index;
    }

    private async Task<StatusListRecord> GetOrCreateStatusListAsync(string issuerDid)
    {
        var statusList = await _db.StatusLists.FindAsync(issuerDid);
        if (statusList != null) return statusList;

        // Create a new status list with 16K entries (2KB bitstring)
        var size = 16384;
        var bytes = new byte[size / 8];
        statusList = new StatusListRecord
        {
            IssuerDid = issuerDid,
            EncodedList = Convert.ToBase64String(bytes),
            Size = size,
            NextIndex = 0
        };
        _db.StatusLists.Add(statusList);
        await _db.SaveChangesAsync();
        return statusList;
    }

    private static void SetBit(StatusListRecord statusList, int index, bool value)
    {
        var bytes = Convert.FromBase64String(statusList.EncodedList);
        var byteIndex = index / 8;
        var bitIndex = index % 8;

        if (byteIndex >= bytes.Length) return;

        if (value)
            bytes[byteIndex] |= (byte)(1 << bitIndex);
        else
            bytes[byteIndex] &= (byte)~(1 << bitIndex);

        statusList.EncodedList = Convert.ToBase64String(bytes);
    }
}
