namespace imm_check_server_example.Models {
    public class ImmDocI20 {
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
}


/*

CREATE TABLE "I20Program" (
	"recnum"	int IDENTITY(1, 1) NOT NULL,
	"idnumber"	int NOT NULL,
	"sevisid"	nvarchar(11),
	"status"	nvarchar(2),
	"eduLevel"	nvarchar(2),
	"eduComments"	nvarchar(500),
	"primaryMajor"	nvarchar(10),
	"secondMajor"	nvarchar(10),
	"minor"	nvarchar(10),
	"lengthOfStudy"	nvarchar(2),
	"prgStartDate"	datetime,
	"prgEndDate"	datetime,
	"engRequired"	bit NOT NULL,
	"engRequirementsMet"	bit NOT NULL,
	"engNotRequired"	nvarchar(500),
	"datestamp"	datetime NOT NULL,
	"institutionalKey"	nvarchar(100),
	"issDate"	datetime

*/