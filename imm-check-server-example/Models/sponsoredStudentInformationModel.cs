namespace imm_check_server_example.Models {
    public class SponsoredStudentInformation {
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
    }
}      
