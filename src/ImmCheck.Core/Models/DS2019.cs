using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImmCheck.Core.Models;

[Table("ds2019info")]
public class DS2019
{
    [Key]
    public long recnum { get; set; }
    public long idnumber { get; set; }
    public string? sponsor { get; set; }
    public string? orgCode { get; set; }
    public string? sevisid { get; set; }
    public string? startProgram { get; set; }
    public string? endProgram { get; set; }
    public string? official { get; set; }
    public string? street1 { get; set; }
    public string? street2 { get; set; }
    public string? city { get; set; }
    public string? state { get; set; }
    public string? postal { get; set; }
    public string? phone { get; set; }
    public string? email { get; set; }
    public string? directBill { get; set; }
    public string? datestamp { get; set; }
    public string? ipfsCID { get; set; }
}
