IF OBJECT_ID('XIMatrixTransaction_T', 'U') IS NOT NULL DROP TABLE dbo.XIMatrixTransaction_T

	
CREATE TABLE [XIMatrixTransaction_T]
(
	[iActionType] INT NULL,
	[zXCrtdBy] VARCHAR(15) NULL,
	[zXCrtdWhn] datetime NULL,
	[zXUpdtdBy] VARCHAR(15) NULL,
	[zXUpdtdWhn] datetime NULL,
	[izXDeleted] INT NULL,
	[sHierarchy] VARCHAR(256) NULL,
	[XIGUID] VARCHAR(128) NULL,
	[iItemInstanceID] INT NULL,
	[ID] INT NOT NULL,
	[sName] VARCHAR(256) NULL,
	[sDescription] VARCHAR(256) NULL,
	[iStatus] INT NULL,
	[refMatrixActionID] bigint NULL,
	[OIID] bigint NULL,
	[ODID] bigint NULL,
	[FKsUserID] VARCHAR(32) NULL,
	[sCode] VARCHAR(32) NULL,
	[sEmail] VARCHAR(64) NULL,
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)