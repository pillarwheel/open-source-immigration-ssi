using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImmCheck.Core.Models;

[Table("sevisI20Financial")]
public class FinancialSupport
{
    [Key]
    public long recnum { get; set; }
    public long idnumber { get; set; }
    public string? sevisid { get; set; }
    public string? academicTerm { get; set; }
    public int? tuition { get; set; }
    public int? livingExpenses { get; set; }
    public int? dependentExp { get; set; }
    public int? otherExpAmount { get; set; }
    public string? otherExpDesc { get; set; }
    public int? personalFunds { get; set; }
    public int? schoolFundsAmount { get; set; }
    public string? schoolFundsDesc { get; set; }
    public int? otherFundsAmount { get; set; }
    public string? otherFundsDesc { get; set; }
    public int? employmentFunds { get; set; }
    public string? remarks { get; set; }
    public string? datestamp { get; set; }
    public string? ipfsCID { get; set; }
}
