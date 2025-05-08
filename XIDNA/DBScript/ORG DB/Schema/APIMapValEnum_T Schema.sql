IF OBJECT_ID('APIMapValEnum_T', 'U') IS NOT NULL DROP TABLE dbo.APIMapValEnum_T

	
CREATE TABLE [APIMapValEnum_T]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[FKiMapValID] INT NULL,
	[sFrom] VARCHAR(100) NULL,
	[sTo] VARCHAR(100) NULL,
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)