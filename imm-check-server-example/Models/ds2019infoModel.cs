namespace imm_check_server_example.Models {
    public class ds2019info {

        public long recnum { get; set; }
        public long idnumber { get; set; }
        public string sponsor { get; set; }
        public string orgCode { get; set; }
        public string sevisid { get; set; }
        public string startProgram { get; set; }
        public string endProgram { get; set; }
        public string official { get; set; }
        public string street1 { get; set; }
        public string street2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string postal { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string directBill { get; set; }
        public string datestamp { get; set; }

    }
}

// [ds2019info]
// ************
// 	[recnum] [int] IDENTITY(1,1) NOT NULL,
// 	[idnumber] [int] NOT NULL,
// 	[sponsor] [nvarchar](200) NULL,
// 	[orgCode] [nvarchar](20) NULL,
// 	[sevisid] [nvarchar](11) NULL,
// 	[startProgram] [datetime] NULL,
// 	[endProgram] [datetime] NULL,
// 	[official] [nvarchar](200) NULL,
// 	[street1] [nvarchar](255) NULL,
// 	[street2] [nvarchar](255) NULL,
// 	[city] [nvarchar](60) NULL,
// 	[state] [nvarchar](2) NULL,
// 	[postal] [nvarchar](20) NULL,
// 	[phone] [nvarchar](20) NULL,
// 	[email] [nvarchar](255) NULL,
// 	[directBill] [bit] NULL,
// 	[datestamp] [datetime] NOT NULL)