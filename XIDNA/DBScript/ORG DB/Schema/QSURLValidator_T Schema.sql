IF OBJECT_ID('QSURLValidator_T', 'U') IS NOT NULL DROP TABLE dbo.QSURLValidator_T

	
CREATE TABLE [QSURLValidator_T]
(
	[ID] INT NULL,
	[sHierarchy] VARCHAR(256) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[sName] VARCHAR(256) NULL,
	[sDescription] VARCHAR(256) NULL,
	[sCode] VARCHAR(256) NULL,
	[iStatus] INT NULL,
	[iType] INT NULL,
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate()),
	[FKiQSIXIGUID] INT NULL,
	[sInstanceXIGUID] VARCHAR(256) NULL,
	[iSecurity] INT NULL,
	[FKiBrandIDXIGUID] UNIQUEIDENTIFIER NULL,
	[FKiBOIID] INT NULL,
	[FKiBODID] INT NULL,
	[FKsPCXIGUID] VARCHAR(256) NULL,
	[refTypeIDXIGUID] FLOAT NULL
)