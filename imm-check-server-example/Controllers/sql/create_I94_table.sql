
CREATE TABLE [dbo].[I94](
	[recnum] [int] IDENTITY(1,1) NOT NULL,
	[idnumber] [int] NOT NULL,
	[i94num] [nvarchar](11) NULL,
	[i94poe] [nvarchar](5) NULL,
	[i94iss] [datetime] NULL,
	[i94exp] [datetime] NULL,
	[i94expds] [bit] NOT NULL,
	[datestamp] [datetime] NOT NULL,
 CONSTRAINT [PK_I94] PRIMARY KEY CLUSTERED 
(
	[recnum] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 20) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[I94] ADD  CONSTRAINT [DF_I94_i94expds]  DEFAULT (0) FOR [i94expds]
GO

ALTER TABLE [dbo].[I94] ADD  CONSTRAINT [DF_I94_datestamp]  DEFAULT (getdate()) FOR [datestamp]
GO
