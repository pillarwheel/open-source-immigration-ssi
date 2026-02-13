using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using imm_check_server_example.Data;
using imm_check_server_example.Models;

namespace imm_check_server_example.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DatabaseController : ControllerBase
{
    private readonly AppDbContext _db;

    public DatabaseController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<ImmDocCollection>> GetData()
    {
        var collection = new ImmDocCollection
        {
            immDocI20 = await _db.I20Programs.ToListAsync(),
            immDocDS2019info = await _db.DS2019Infos.ToListAsync(),
            immDocI94 = await _db.I94s.ToListAsync(),
            immDocPassport = await _db.Passports.ToListAsync(),
            immDocVisaInfo = await _db.VisaInfos.ToListAsync(),
            sponsoredStudentInformation = await _db.SponsoredStudentInformation.ToListAsync()
        };

        return Ok(collection);
    }

    [HttpGet("i20")]
    public async Task<ActionResult<List<ImmDocI20>>> GetI20s()
    {
        return await _db.I20Programs.ToListAsync();
    }

    [HttpGet("ds2019")]
    public async Task<ActionResult<List<ImmDocDS2019info>>> GetDS2019s()
    {
        return await _db.DS2019Infos.ToListAsync();
    }

    [HttpGet("i94")]
    public async Task<ActionResult<List<ImmDocI94>>> GetI94s()
    {
        return await _db.I94s.ToListAsync();
    }

    [HttpGet("passport")]
    public async Task<ActionResult<List<ImmDocPassport>>> GetPassports()
    {
        return await _db.Passports.ToListAsync();
    }

    [HttpGet("visa")]
    public async Task<ActionResult<List<ImmDocVisaInfo>>> GetVisaInfos()
    {
        return await _db.VisaInfos.ToListAsync();
    }

    [HttpGet("sponsored-student")]
    public async Task<ActionResult<List<SponsoredStudentInformation>>> GetSponsoredStudents()
    {
        return await _db.SponsoredStudentInformation.ToListAsync();
    }
}
