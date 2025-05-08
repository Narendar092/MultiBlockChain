IF OBJECT_ID('enumMake_T', 'U') IS NOT NULL DROP TABLE dbo.enumMake_T

	
CREATE TABLE [enumMake_T]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[sName] NVARCHAR(255) NULL,
	[izXDeleted] INT NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)