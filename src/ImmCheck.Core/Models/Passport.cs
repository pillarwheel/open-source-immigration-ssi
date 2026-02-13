using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImmCheck.Core.Models;

[Table("passport")]
public class Passport
{
    [Key]
    public long recnum { get; set; }
    public long idnumber { get; set; }
    public string? givenName { get; set; }
    public string? lastname { get; set; }
    public string? cpass { get; set; }
    public string? passnum { get; set; }
    public string? passiss { get; set; }
    public string? passexp { get; set; }
    public string? datestamp { get; set; }
    public string? ipfsCID { get; set; }
}
