using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImmCheck.Core.Models;

[Table("I20Program")]
public class I20
{
    [Key]
    public long recnum { get; set; }
    public long idnumber { get; set; }
    public string? sevisid { get; set; }
    public string? status { get; set; }
    public string? eduLevel { get; set; }
    public string? eduComments { get; set; }
    public string? primaryMajor { get; set; }
    public string? secondMajor { get; set; }
    public string? minor { get; set; }
    public string? lengthOfStudy { get; set; }
    public string? prgStartDate { get; set; }
    public string? prgEndDate { get; set; }
    public string? engRequired { get; set; }
    public string? engRequirementsMet { get; set; }
    public string? engNotRequired { get; set; }
    public string? datestamp { get; set; }
    public string? institutionalKey { get; set; }
    public string? issDate { get; set; }
    public string? ipfsCID { get; set; }
}
