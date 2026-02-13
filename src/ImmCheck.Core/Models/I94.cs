using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImmCheck.Core.Models;

[Table("I94")]
public class I94
{
    [Key]
    public long recnum { get; set; }
    public long idnumber { get; set; }
    public string? i94num { get; set; }
    public string? i94poe { get; set; }
    public string? i94iss { get; set; }
    public string? i94exp { get; set; }
    public string? i94expds { get; set; }
    public string? datestamp { get; set; }
    public string? ipfsCID { get; set; }
}
