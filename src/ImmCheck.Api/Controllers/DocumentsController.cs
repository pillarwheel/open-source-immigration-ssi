using Microsoft.AspNetCore.Mvc;
using ImmCheck.Core.Interfaces;
using ImmCheck.Core.Models;

namespace ImmCheck.Api.Controllers;

[Route("api/documents/[controller]")]
[ApiController]
public class I20Controller : ControllerBase
{
    private readonly IDocumentRepository<I20> _repo;
    public I20Controller(IDocumentRepository<I20> repo) => _repo = repo;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<I20>>> GetAll() =>
        Ok(await _repo.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<I20>> GetById(long id)
    {
        var item = await _repo.GetByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<I20>> Create(I20 entity)
    {
        var created = await _repo.CreateAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = created.recnum }, created);
    }
}

[Route("api/documents/[controller]")]
[ApiController]
public class DS2019Controller : ControllerBase
{
    private readonly IDocumentRepository<DS2019> _repo;
    public DS2019Controller(IDocumentRepository<DS2019> repo) => _repo = repo;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DS2019>>> GetAll() =>
        Ok(await _repo.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<DS2019>> GetById(long id)
    {
        var item = await _repo.GetByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<DS2019>> Create(DS2019 entity)
    {
        var created = await _repo.CreateAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = created.recnum }, created);
    }
}

[Route("api/documents/[controller]")]
[ApiController]
public class I94Controller : ControllerBase
{
    private readonly IDocumentRepository<I94> _repo;
    public I94Controller(IDocumentRepository<I94> repo) => _repo = repo;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<I94>>> GetAll() =>
        Ok(await _repo.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<I94>> GetById(long id)
    {
        var item = await _repo.GetByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<I94>> Create(I94 entity)
    {
        var created = await _repo.CreateAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = created.recnum }, created);
    }
}

[Route("api/documents/[controller]")]
[ApiController]
public class PassportController : ControllerBase
{
    private readonly IDocumentRepository<Passport> _repo;
    public PassportController(IDocumentRepository<Passport> repo) => _repo = repo;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Passport>>> GetAll() =>
        Ok(await _repo.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<Passport>> GetById(long id)
    {
        var item = await _repo.GetByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<Passport>> Create(Passport entity)
    {
        var created = await _repo.CreateAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = created.recnum }, created);
    }
}

[Route("api/documents/[controller]")]
[ApiController]
public class VisaController : ControllerBase
{
    private readonly IDocumentRepository<VisaInfo> _repo;
    public VisaController(IDocumentRepository<VisaInfo> repo) => _repo = repo;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<VisaInfo>>> GetAll() =>
        Ok(await _repo.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<VisaInfo>> GetById(long id)
    {
        var item = await _repo.GetByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<VisaInfo>> Create(VisaInfo entity)
    {
        var created = await _repo.CreateAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = created.recnum }, created);
    }
}

[Route("api/documents/financial-support")]
[ApiController]
public class FinancialSupportController : ControllerBase
{
    private readonly IDocumentRepository<FinancialSupport> _repo;
    public FinancialSupportController(IDocumentRepository<FinancialSupport> repo) => _repo = repo;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FinancialSupport>>> GetAll() =>
        Ok(await _repo.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<FinancialSupport>> GetById(long id)
    {
        var item = await _repo.GetByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<FinancialSupport>> Create(FinancialSupport entity)
    {
        var created = await _repo.CreateAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = created.recnum }, created);
    }
}

[Route("api/documents/sponsored-student")]
[ApiController]
public class SponsoredStudentController : ControllerBase
{
    private readonly IDocumentRepository<SponsoredStudentInfo> _repo;
    public SponsoredStudentController(IDocumentRepository<SponsoredStudentInfo> repo) => _repo = repo;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SponsoredStudentInfo>>> GetAll() =>
        Ok(await _repo.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<SponsoredStudentInfo>> GetById(long id)
    {
        var item = await _repo.GetByIdAsync(id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<SponsoredStudentInfo>> Create(SponsoredStudentInfo entity)
    {
        var created = await _repo.CreateAsync(entity);
        return CreatedAtAction(nameof(GetById), new { id = created.recnum }, created);
    }
}
