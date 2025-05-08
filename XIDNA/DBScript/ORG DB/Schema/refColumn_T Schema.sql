IF OBJECT_ID('refColumn_T', 'U') IS NOT NULL DROP TABLE dbo.refColumn_T

	
CREATE TABLE [refColumn_T]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[sName] VARCHAR(50) Not NULL,
	[iColNo] bigint NOT NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)