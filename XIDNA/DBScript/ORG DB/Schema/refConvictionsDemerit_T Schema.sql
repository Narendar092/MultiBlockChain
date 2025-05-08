IF OBJECT_ID('refConvictionsDemerit_T', 'U') IS NOT NULL DROP TABLE dbo.refConvictionsDemerit_T

	
CREATE TABLE [refConvictionsDemerit_T]
(
	[sName] NVARCHAR(255) NULL,
	[sValue] float NULL,
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[zXCrtdBy] VARCHAR(128) NULL,
	[zXCrtdWhn] datetime NULL,
	[zXUpdtdBy] VARCHAR(128) NULL,
	[zXUpdtdWhn] datetime NULL,
	[FKiProductID] INT NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)