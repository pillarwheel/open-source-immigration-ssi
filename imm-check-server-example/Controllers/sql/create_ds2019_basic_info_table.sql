CREATE TABLE [dbo].[ds2019info](
	[recnum] [int] IDENTITY(1,1) NOT NULL,
	[idnumber] [int] NOT NULL,
	[sponsor] [nvarchar](200) NULL,
	[orgCode] [nvarchar](20) NULL,
	[sevisid] [nvarchar](11) NULL,
	[startProgram] [datetime] NULL,
	[endProgram] [datetime] NULL,
	[official] [nvarchar](200) NULL,
	[street1] [nvarchar](255) NULL,
	[street2] [nvarchar](255) NULL,
	[city] [nvarchar](60) NULL,
	[state] [nvarchar](2) NULL,
	[postal] [nvarchar](20) NULL,
	[phone] [nvarchar](20) NULL,
	[email] [nvarchar](255) NULL,
	[directBill] [bit] NULL,
	[datestamp] [datetime] NOT NULL,
 CONSTRAINT [PK_sevisDS2019OtherSponsor] PRIMARY KEY CLUSTERED 
(
	[recnum] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 20) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ds2019info] ADD  CONSTRAINT [DF_ds2019info_directBill]  DEFAULT ((0)) FOR [directBill]
GO

ALTER TABLE [dbo].[ds2019info] ADD  CONSTRAINT [DF_ds2019info_datestamp]  DEFAULT (getdate()) FOR [datestamp]
GO


