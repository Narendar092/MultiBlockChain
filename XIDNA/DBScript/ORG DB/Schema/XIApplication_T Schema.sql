IF OBJECT_ID('XIApplication_T', 'U') IS NOT NULL DROP TABLE dbo.XIApplication_T

	
CREATE TABLE [XIApplication_T]
(
	[bNannoApp] bit NULL,
	[zXCrtdBy] VARCHAR(15) Not NULL,
	[zXCrtdWhn] datetime NOT NULL,
	[zXUpdtdBy] VARCHAR(15) Not NULL,
	[zXUpdtdWhn] datetime NOT NULL,
	[izXDeleted] INT NULL,
	[sHierarchy] VARCHAR(256) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate()),
	[sUserName] VARCHAR(128) NULL
)