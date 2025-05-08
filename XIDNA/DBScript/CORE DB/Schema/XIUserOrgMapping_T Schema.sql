IF OBJECT_ID('XIUserOrgMapping_T', 'U') IS NOT NULL DROP TABLE dbo.XIUserOrgMapping_T

	
CREATE TABLE [XIUserOrgMapping_T]
(
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate()),
	[zXCrtdBy] VARCHAR(32) NULL,
	[iType] INT NULL,
	[FKiOrgID] INT NULL,
	[zXUpdtdWhn] datetime NULL,
	[sDescription] VARCHAR(512) NULL,
	[iStatus] INT NULL,
	[FKiRoleID] INT NULL,
	[zXCrtdWhn] datetime NULL,
	[FKiUserID] INT NULL,
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[sCode] VARCHAR(32) NULL,
	[zXUpdtdBy] VARCHAR(32) NULL,
	[izXDeleted] INT NULL,
	[sName] VARCHAR(64) NULL
)