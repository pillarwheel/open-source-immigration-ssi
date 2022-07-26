
CREATE TABLE [I94](
	[recnum] [int] IDENTITY(1,1) NOT NULL,
	[idnumber] [int] NOT NULL,
	[i94num] [nvarchar](11) NULL,
	[i94poe] [nvarchar](5) NULL,
	[i94iss] [datetime] NULL,
	[i94exp] [datetime] NULL,
	[i94expds] [bit] NOT NULL,
	[datestamp] [datetime] NOT NULL)