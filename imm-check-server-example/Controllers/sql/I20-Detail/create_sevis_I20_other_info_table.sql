CREATE TABLE [dbo].[sevisI20OtherInfo](
	[recnum] [int] IDENTITY(1,1) NOT NULL,
	[idnumber] [int] NOT NULL,
	[sevisid] [nvarchar](11) NULL,
	[currentSessionStart] [datetime] NULL,
	[currentSessionEnd] [datetime] NULL,
	[nextSessionStart] [datetime] NULL,
	[commuter] [bit] NOT NULL,
	[admissionNumber] [nvarchar](11) NULL,
	[datestamp] [datetime] NOT NULL,
	[lastSession] [bit] NULL,
	[statusCancelReason] [nvarchar](2) NULL,
	[statusTerminateReason] [nvarchar](2) NULL,
	[statusRemarks] [nvarchar](500) NULL,
	[capGapStatus] [nvarchar](2) NULL,
	[disciplineExpl] [nvarchar](500) NULL,
	[levelRemarks] [nvarchar](500) NULL,
	[programRemarks] [nvarchar](500) NULL,
	[extExplanation] [nvarchar](500) NULL,
	[registrationRemarks] [nvarchar](500) NULL,
	[reprintReason] [nvarchar](2) NULL,
	[reprintRemarks] [nvarchar](500) NULL,
	[terminateOtherReason] [nvarchar](500) NULL,
	[studyResearchAbroad] [bit] NULL,
	[thesisDissertation] [bit] NULL,
	[registrationEligibility] [nvarchar](100) NULL,
 CONSTRAINT [PK_sevisI20OtherInfo] PRIMARY KEY CLUSTERED 
(
	[recnum] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[sevisI20OtherInfo] ADD  CONSTRAINT [DF_sevisI20OtherInfo_commuter]  DEFAULT ((0)) FOR [commuter]
GO

ALTER TABLE [dbo].[sevisI20OtherInfo] ADD  CONSTRAINT [DF_sevisI20OtherInfo_datestamp]  DEFAULT (getdate()) FOR [datestamp]
GO

