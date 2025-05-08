IF OBJECT_ID('QSURLTemplate_T', 'U') IS NOT NULL DROP TABLE dbo.QSURLTemplate_T

	
CREATE TABLE [QSURLTemplate_T]
(
	[ID] INT NULL,
	[sHierarchy] VARCHAR(256) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[sName] VARCHAR(256) NULL,
	[sDescription] VARCHAR(256) NULL,
	[sCode] VARCHAR(256) NULL,
	[iStatus] INT NULL,
	[iType] INT NULL,
	[FKiDefaultBrandXIGUID] INT NULL,
	[sNotValidMesssage] VARCHAR(256) NULL,
	[sErrorMessage] VARCHAR(256) NULL,
	[iSecurity] INT NULL,
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate()),
	[FKsPCGUIDID] VARCHAR(256) NULL
)