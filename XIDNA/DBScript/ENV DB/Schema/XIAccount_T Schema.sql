IF OBJECT_ID('XIAccount_T', 'U') IS NOT NULL DROP TABLE dbo.XIAccount_T

	
CREATE TABLE [XIAccount_T]
(
	[sFromAddress] VARCHAR(512) NULL,
	[XICreatedBy] VARCHAR(15) Not NULL,
	[XICreatedWhen] datetime NOT NULL,
	[XIUpdatedBy] VARCHAR(15) Not NULL,
	[XIUpdatedWhen] datetime NOT NULL,
	[XIDeleted] INT NULL,
	[sHierarchy] VARCHAR(256) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[ID] INT NULL,
	[sName] VARCHAR(256) NULL,
	[sDescription] VARCHAR(256) NULL,
	[sCode] VARCHAR(32) NULL,
	[iType] INT NULL,
	[iStatus] INT NULL,
	[iPort] INT NULL,
	[sSecurity] VARCHAR(256) NULL,
	[sUserName] VARCHAR(512) NULL,
	[sPassword] VARCHAR(512) NULL,
	[sAPIURL] VARCHAR(1024) NULL,
	[FKiAppID] INT NULL,
	[FKiOrgID] INT NULL,
	[FKiAppIDXIGUID] UNIQUEIDENTIFIER NULL
)