IF OBJECT_ID('EnumXiActionTypes_T', 'U') IS NOT NULL DROP TABLE dbo.EnumXiActionTypes_T

	
CREATE TABLE [EnumXiActionTypes_T]
(
	[id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[sName] VARCHAR(256) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)