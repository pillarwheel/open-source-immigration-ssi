CREATE TABLE [dbo].[sevisI20Financial](
	[recnum] [int] IDENTITY(1,1) NOT NULL,
	[idnumber] [int] NOT NULL,
	[sevisid] [nvarchar](11) NULL,
	[academicTerm] [nvarchar](10) NULL,
	[tuition] [int] NULL,
	[livingExpenses] [int] NULL,
	[dependentExp] [int] NULL,
	[otherExpAmount] [int] NULL,
	[otherExpDesc] [nvarchar](500) NULL,
	[personalFunds] [int] NULL,
	[schoolFundsAmount] [int] NULL,
	[schoolFundsDesc] [nvarchar](500) NULL,
	[otherFundsAmount] [int] NULL,
	[otherFundsDesc] [nvarchar](500) NULL,
	[employmentFunds] [int] NULL,
	[remarks] [nvarchar](500) NULL,
	[datestamp] [datetime] NOT NULL,
 CONSTRAINT [PK_sevisI20Financial] PRIMARY KEY CLUSTERED 
(
	[recnum] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 20) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[sevisI20Financial] ADD  CONSTRAINT [DF_sevisI20Financial_datestamp]  DEFAULT (getdate()) FOR [datestamp]
GO

ALTER TABLE [dbo].[sevisI20Financial]  WITH CHECK ADD  CONSTRAINT [FK_sevisI20Financial_jbInternational] FOREIGN KEY([idnumber])
REFERENCES [dbo].[jbInternational] ([idnumber])
GO

ALTER TABLE [dbo].[sevisI20Financial] CHECK CONSTRAINT [FK_sevisI20Financial_jbInternational]
GO


