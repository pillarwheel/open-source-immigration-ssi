namespace ImmCheck.Core.SSI.Credentials;

public interface ICredentialRepository
{
    Task<IssuedCredentialRecord> StoreAsync(IssuedCredentialRecord record);
    Task<IssuedCredentialRecord?> GetByIdAsync(string credentialId);
    Task<IEnumerable<IssuedCredentialRecord>> GetBySubjectAsync(string subjectDid);
    Task<IEnumerable<IssuedCredentialRecord>> GetByIssuerAsync(string issuerDid);
    Task<bool> RevokeAsync(string credentialId);
    Task<StatusListRecord?> GetStatusListAsync(string issuerDid);
    Task<StatusListRecord> UpdateStatusListAsync(StatusListRecord statusList);
    Task<int> AllocateStatusListIndexAsync(string issuerDid);
}
