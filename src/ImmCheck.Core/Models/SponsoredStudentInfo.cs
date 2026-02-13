using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImmCheck.Core.Models;

[Table("sponsoredStudentInformation")]
public class SponsoredStudentInfo
{
    [Key]
    public long recnum { get; set; }
    public long idnumber { get; set; }
    public string? sponsor { get; set; }
    public string? fundingSponsor { get; set; }
    public string? advisorName { get; set; }
    public string? advisorEmail { get; set; }
    public string? advisorPhone { get; set; }
    public string? isSponsoredStudent { get; set; }
    public string? datestamp { get; set; }
    public string? isFeeExempt { get; set; }
    public string? isUSGovSponsored { get; set; }
    public string? needsFinancialDocs { get; set; }
    public string? financialDocsExpDate { get; set; }
    public string? ipfsCID { get; set; }
}
