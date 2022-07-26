CREATE TABLE [dbo].[sponsoredStudentInformation](
	[recnum] [int] IDENTITY(1,1) NOT NULL,
	[idnumber] [int] NOT NULL,
	[sponsor] [nvarchar](200) NOT NULL,
	[fundingSponsor] [nvarchar](200) NULL,
	[advisorName] [nvarchar](100) NULL,
	[advisorEmail] [nvarchar](255) NULL,
	[advisorPhone] [nvarchar](20) NULL,
	[isSponsoredStudent] [bit] NOT NULL,
	[datestamp] [datetime] NOT NULL,
	[isFeeExempt] [bit] NULL,
	[isUSGovSponsored] [bit] NULL,
	[needsFinancialDocs] [bit] NULL,
	[financialDocsExpDate] [datetime] NULL,
 CONSTRAINT [PK_sponsoredStudentInformation] PRIMARY KEY CLUSTERED 
(
	[recnum] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 20) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[sponsoredStudentInformation] ADD  CONSTRAINT [DF_sponsoredStudentInformation_isSponsoredStudent]  DEFAULT ((0)) FOR [isSponsoredStudent]
GO

ALTER TABLE [dbo].[sponsoredStudentInformation] ADD  CONSTRAINT [DF_sponsoredStudentInformation_datestamp]  DEFAULT (getdate()) FOR [datestamp]
GO
