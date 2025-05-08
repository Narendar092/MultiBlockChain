IF OBJECT_ID('APIMap_T', 'U') IS NOT NULL DROP TABLE dbo.APIMap_T

	
CREATE TABLE [APIMap_T]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[Name] VARCHAR(100) NULL,
	[sCode] VARCHAR(50) NULL,
	[FKiProductID] INT NULL,
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)