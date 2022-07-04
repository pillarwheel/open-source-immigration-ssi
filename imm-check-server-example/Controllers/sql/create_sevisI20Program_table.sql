USE [InternationalServices]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[I20Program](
	[recnum] [int] IDENTITY(1,1) NOT NULL,
	[idnumber] [int] NOT NULL,
	[sevisid] [nvarchar](11) NULL,
	[status] [nvarchar](2) NULL,
	[eduLevel] [nvarchar](2) NULL,
	[eduComments] [nvarchar](500) NULL,
	[primaryMajor] [nvarchar](10) NULL,
	[secondMajor] [nvarchar](10) NULL,
	[minor] [nvarchar](10) NULL,
	[lengthOfStudy] [nvarchar](2) NULL,
	[prgStartDate] [datetime] NULL,
	[prgEndDate] [datetime] NULL,
	[engRequired] [bit] NOT NULL,
	[engRequirementsMet] [bit] NOT NULL,
	[engNotRequired] [nvarchar](500) NULL,
	[datestamp] [datetime] NOT NULL,
	[institutionalKey] [nvarchar](100) NULL,
	[issDate] [datetime] NULL,
 CONSTRAINT [PK_sevisI20Program] PRIMARY KEY CLUSTERED 
(
	[recnum] ASC,
	[idnumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 20) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[I20Program] ADD  CONSTRAINT [DF_I20Program_engRequired]  DEFAULT (0) FOR [engRequired]
GO

ALTER TABLE [dbo].[I20Program] ADD  CONSTRAINT [DF_I20Program_engRequirementsMet]  DEFAULT (0) FOR [engRequirementsMet]
GO

ALTER TABLE [dbo].[I20Program] ADD  CONSTRAINT [DF_I20Program_datestamp]  DEFAULT (getdate()) FOR [datestamp]
GO

