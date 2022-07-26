CREATE TABLE [dbo].[sevisDS2019Ext](
	[recnum] [int] IDENTITY(1,1) NOT NULL,
	[idnumber] [int] NOT NULL,
	[sevisid] [nvarchar](11) NULL,
	[datestamp] [datetime] NOT NULL,
	[enrollNotRequired] [bit] NULL,
	[statusReason] [nvarchar](5) NULL,
	[statusRemarks] [nvarchar](500) NULL,
	[reprintReason] [nvarchar](2) NULL,
	[reprintRemarks] [nvarchar](500) NULL,
	[programRemarks] [nvarchar](500) NULL,
	[statusTerminateReason] [nvarchar](6) NULL,
	[terminateStatusOtherRemarks] [nvarchar](500) NULL,
	[homeCountryPositionCode] [nvarchar](3) NULL,
	[reprintOtherRemarks] [nvarchar](500) NULL,
	[matriculation] [nvarchar](2) NULL,
	[infractionType] [nvarchar](3) NULL,
	[occupationCategory] [nvarchar](2) NULL,
	[monthsInStatus] [int] NULL,
	[has2YearHomeResReq] [bit] NULL,
	[waiverGranted] [bit] NULL,
 CONSTRAINT [PK_sevisDS2019Ext] PRIMARY KEY CLUSTERED 
(
	[recnum] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 20) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[sevisDS2019Ext] ADD  CONSTRAINT [DF_sevisDS2019Ext_datestamp]  DEFAULT (getdate()) FOR [datestamp]
GO

ALTER TABLE [dbo].[sevisDS2019Ext] ADD  CONSTRAINT [DF_sevisDS2019Ext_monthsInStatus]  DEFAULT ((0)) FOR [monthsInStatus]
GO

ALTER TABLE [dbo].[sevisDS2019Ext] ADD  CONSTRAINT [DF_sevisDS2019Ext_has2YearHomeResReq]  DEFAULT ((0)) FOR [has2YearHomeResReq]
GO

ALTER TABLE [dbo].[sevisDS2019Ext] ADD  CONSTRAINT [DF_sevisDS2019Ext_waiverGranted]  DEFAULT ((0)) FOR [waiverGranted]
GO

ALTER TABLE [dbo].[sevisDS2019Ext]  WITH CHECK ADD  CONSTRAINT [FK_sevisDS2019Ext_jbInternational] FOREIGN KEY([idnumber])
REFERENCES [dbo].[jbInternational] ([idnumber])
GO

ALTER TABLE [dbo].[sevisDS2019Ext] CHECK CONSTRAINT [FK_sevisDS2019Ext_jbInternational]
GO


