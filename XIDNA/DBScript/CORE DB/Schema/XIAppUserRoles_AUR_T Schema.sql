IF OBJECT_ID('XIAppUserRoles_AUR_T', 'U') IS NOT NULL DROP TABLE dbo.XIAppUserRoles_AUR_T

	
CREATE TABLE [XIAppUserRoles_AUR_T]
(
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate()),
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[zXUpdtdBy] VARCHAR(32) NULL,
	[izXDeleted] INT NULL,
	[UserID] INT NULL,
	[zXCrtdBy] VARCHAR(32) NULL,
	[zXUpdtdWhn] datetime NULL,
	[RoleID] INT NULL,
	[zXCrtdWhn] datetime NULL,
	[UserIDXIGUID] UNIQUEIDENTIFIER NULL,
	[RoleIDXIGUID] UNIQUEIDENTIFIER NULL
)