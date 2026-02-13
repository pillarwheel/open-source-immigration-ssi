using System.ComponentModel.DataAnnotations;

namespace ImmCheck.Core.Models;

public class Institution
{
    [Key]
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? SchoolCode { get; set; }
    public string? Did { get; set; }
}
