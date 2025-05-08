IF OBJECT_ID('XIAppRoles_AR_T', 'U') IS NOT NULL DROP TABLE dbo.XIAppRoles_AR_T

	
CREATE TABLE [XIAppRoles_AR_T]
(
	[FKiOrgID] INT NULL,
	[iLayoutID] INT NULL default((0)),
	[bDBAccess] bit NOT NULL default((0)),
	[StatusTypeID] INT NOT NULL default((0)),
	[CreatedBy] INT NOT NULL default((0)),
	[CreatedTime] datetime NOT NULL default(getdate()),
	[UpdatedBy] INT NOT NULL default((0)),
	[UpdatedTime] datetime NOT NULL default(getdate()),
	[bSignalR] bit NULL default((0)),
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate()),
	[iThemeID] INT NULL,
	[iParentIDXIGUID] UNIQUEIDENTIFIER NULL,
	[izXDeleted] INT NULL,
	[sRoleName] NVARCHAR(MAX) NOT NULL,
	[zXUpdtdBy] VARCHAR(32) NULL,
	[zXUpdtdWhn] datetime NULL,
	[RoleID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[zXCrtdBy] VARCHAR(32) NULL,
	[zXCrtdWhn] datetime NULL,
	[iThemeIDXIGUID] UNIQUEIDENTIFIER NULL,
	[UpdatedBySYSID] VARCHAR(512) NULL,
	[CreatedBySYSID] VARCHAR(512) NULL,
	[iParentID] INT NULL default((0)),
	[iLayoutIDXIGUID] UNIQUEIDENTIFIER NULL,
	[bOrgLock] bit NULL default((0))

)