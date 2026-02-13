using Microsoft.AspNetCore.Identity;

namespace ImmCheck.Core.Models;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public long? InstitutionId { get; set; }
    public Institution? Institution { get; set; }
}
