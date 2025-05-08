IF OBJECT_ID('APIMapVal_T', 'U') IS NOT NULL DROP TABLE dbo.APIMapVal_T

	
CREATE TABLE [APIMapVal_T]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[sBO] VARCHAR(100) NULL,
	[Attribute] VARCHAR(100) NULL,
	[Transform] VARCHAR(MAX) NULL,
	[FKiMapID] INT NULL,
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)