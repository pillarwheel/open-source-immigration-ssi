using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImmCheck.Core.Models;

[Table("visaInfo")]
public class VisaInfo
{
    [Key]
    public long recnum { get; set; }
    public long idnumber { get; set; }
    public string? status { get; set; }
    public string? visastamp { get; set; }
    public string? stampnum { get; set; }
    public string? controlnumber { get; set; }
    public string? stampplace { get; set; }
    public string? stampent { get; set; }
    public string? stampiss { get; set; }
    public string? stampexp { get; set; }
    public string? datestamp { get; set; }
    public string? ipfsCID { get; set; }
}
