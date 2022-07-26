CREATE TABLE [dbo].[passport](
	[recnum] [int] IDENTITY(1,1) NOT NULL,
	[idnumber] [int] NOT NULL,
	[givenName] [nvarchar](101) NULL,
	[lastname] [nvarchar](50) NULL,
	[cpass] [nvarchar](5) NULL,
	[passnum] [nvarchar](255) NULL,
	[passiss] [datetime] NULL,
	[passexp] [datetime] NULL,
	[datestamp] [datetime] NOT NULL,
 CONSTRAINT [PK_passport] PRIMARY KEY CLUSTERED 
(
	[recnum] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 20) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[passport] ADD  CONSTRAINT [DF_passport_datestamp]  DEFAULT (getdate()) FOR [datestamp]
GO

ALTER TABLE [dbo].[passport]  WITH CHECK ADD  CONSTRAINT [FK_passport_jbInternational] FOREIGN KEY([idnumber])
REFERENCES [dbo].[jbInternational] ([idnumber])
GO

ALTER TABLE [dbo].[passport] CHECK CONSTRAINT [FK_passport_jbInternational]
GO


