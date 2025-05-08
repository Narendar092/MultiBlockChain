IF OBJECT_ID('XIBOWhiteList_T', 'U') IS NOT NULL DROP TABLE dbo.XIBOWhiteList_T

	
CREATE TABLE [XIBOWhiteList_T]
(
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate()),
	[zXCrtdBy] VARCHAR(15) NULL default((0)),
	[zXCrtdWhn] datetime NULL default(getdate()),
	[zXUpdtdBy] VARCHAR(15) NULL default((0)),
	[zXUpdtdWhn] datetime NULL default(getdate()),
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[sCode] VARCHAR(256) NULL,
	[FKiRoleID] INT NULL,
	[bDelete] bit NULL,
	[FKiOrgID] INT NULL,
	[bCreate] bit NULL,
	[sName] VARCHAR(256) NULL,
	[izXDeleted] INT NULL,
	[bRead] bit NULL,
	[bUNAuthorize] bit NULL,
	[iStatus] INT NULL,
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[FKiAppID] INT NULL,
	[bAction] bit NULL,
	[sHierarchy] VARCHAR(256) NULL,
	[iType] INT NULL,
	[sDescription] VARCHAR(256) NULL,
	[bUpdate] bit NULL,
	[FKiBODID] INT NULL,
	[b1Query] bit NULL,
	[FKiBODIDXIGUID] UNIQUEIDENTIFIER null,
	[FKiRoleIDXIGUID] UNIQUEIDENTIFIER null
)