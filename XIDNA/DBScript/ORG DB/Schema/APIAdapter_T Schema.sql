IF OBJECT_ID('APIAdapter_T', 'U') IS NOT NULL DROP TABLE dbo.APIAdapter_T

	
CREATE TABLE [APIAdapter_T]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[sName] VARCHAR(256) NULL,
	[sKey] VARCHAR(256) NULL,
	[sValue] VARCHAR(256) NULL,
	[bISHeader] bit NULL,
	[bISBody] bit NULL,
	[sAPICall] VARCHAR(256) NULL,
	[FKiProductID] INT NULL,
	[FKiAPIID] INT NULL,
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)