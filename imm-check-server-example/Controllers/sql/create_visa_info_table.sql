CREATE TABLE [dbo].[visaInfo](
	[recnum] [int] IDENTITY(1,1) NOT NULL,
	[idnumber] [int] NOT NULL,
	[status] [nvarchar](10) NULL,
	[visastamp] [nvarchar](10) NULL,
	[stampnum] [nvarchar](255) NULL,
	[controlnumber] [nvarchar](255) NULL,
	[stampplace] [nvarchar](50) NULL,
	[stampent] [nvarchar](5) NULL,
	[stampiss] [datetime] NULL,
	[stampexp] [datetime] NULL,
	[datestamp] [datetime] NOT NULL,
 CONSTRAINT [PK_visaInfo] PRIMARY KEY CLUSTERED 
(
	[recnum] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 20) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[visaInfo] ADD  CONSTRAINT [DF_visaInfo_datestamp]  DEFAULT (getdate()) FOR [datestamp]
GO
