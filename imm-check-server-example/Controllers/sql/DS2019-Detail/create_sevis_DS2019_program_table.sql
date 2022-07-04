CREATE TABLE [dbo].[sevisDS2019Program](
	[recnum] [int] IDENTITY(1,1) NOT NULL,
	[idnumber] [int] NOT NULL,
	[sevisid] [nvarchar](11) NULL,
	[status] [nvarchar](1) NULL,
	[categoryCode] [nvarchar](5) NULL,
	[positionCode] [int] NULL,
	[prgStartDate] [datetime] NULL,
	[prgEndDate] [datetime] NULL,
	[subjectFieldCode] [nvarchar](10) NULL,
	[subjectFieldRemarks] [nvarchar](500) NULL,
	[datestamp] [datetime] NOT NULL,
	[foreignDegreeLevel] [nvarchar](100) NULL,
	[foreignFieldOfStudy] [nvarchar](100) NULL,
	[institutionalKey] [nvarchar](100) NULL,
	[validatedOn] [datetime] NULL,
	[evId] [nvarchar](25) NULL,
	[evRemarks] [nvarchar](1000) NULL,
 CONSTRAINT [PK_sevisDS2019Program] PRIMARY KEY CLUSTERED 
(
	[recnum] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 20) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[sevisDS2019Program] ADD  CONSTRAINT [DF_sevisDS2019Program_datestamp]  DEFAULT (getdate()) FOR [datestamp]
GO

ALTER TABLE [dbo].[sevisDS2019Program]  WITH CHECK ADD  CONSTRAINT [FK_sevisDS2019Program_jbInternational] FOREIGN KEY([idnumber])
REFERENCES [dbo].[jbInternational] ([idnumber])
GO

ALTER TABLE [dbo].[sevisDS2019Program] CHECK CONSTRAINT [FK_sevisDS2019Program_jbInternational]
GO


