CREATE TABLE [dbo].[sevisDS2019Financial](
	[recnum] [int] IDENTITY(1,1) NOT NULL,
	[idnumber] [int] NOT NULL,
	[sevisid] [nvarchar](11) NULL,
	[receivedUSGovFunds] [bit] NOT NULL,
	[programSponsorFunds] [int] NULL,
	[govtOrg1] [nvarchar](5) NULL,
	[govtOrg1Amount] [int] NULL,
	[govtOrg2] [nvarchar](5) NULL,
	[govtOrg2Amount] [int] NULL,
	[internatOrg1] [nvarchar](10) NULL,
	[internatOrg1Amount] [int] NULL,
	[internatOrg2] [nvarchar](10) NULL,
	[internatOrg2Amount] [int] NULL,
	[evGovt] [int] NULL,
	[binationalCommission] [int] NULL,
	[otherOrgNames] [nvarchar](250) NULL,
	[otherOrg] [int] NULL,
	[personalFunds] [int] NULL,
	[remarks] [nvarchar](500) NULL,
	[datestamp] [datetime] NOT NULL,
	[otherGovtOrg1Name] [nvarchar](60) NULL,
	[otherGovtOrg2Name] [nvarchar](60) NULL,
	[otherInternatOrg1Name] [nvarchar](60) NULL,
	[otherInternatOrg2Name] [nvarchar](60) NULL,
 CONSTRAINT [PK_sevisDS2019Financial] PRIMARY KEY CLUSTERED 
(
	[recnum] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 20) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[sevisDS2019Financial] ADD  CONSTRAINT [DF_sevisDS2019Financial_receivedUSGovFunds]  DEFAULT (0) FOR [receivedUSGovFunds]
GO

ALTER TABLE [dbo].[sevisDS2019Financial] ADD  CONSTRAINT [DF_sevisDS2019Financial_datestamp]  DEFAULT (getdate()) FOR [datestamp]
GO

ALTER TABLE [dbo].[sevisDS2019Financial]  WITH CHECK ADD  CONSTRAINT [FK_sevisDS2019Financial_jbInternational] FOREIGN KEY([idnumber])
REFERENCES [dbo].[jbInternational] ([idnumber])
GO

ALTER TABLE [dbo].[sevisDS2019Financial] CHECK CONSTRAINT [FK_sevisDS2019Financial_jbInternational]
GO


