IF OBJECT_ID('XIImportBatch_T', 'U') IS NOT NULL DROP TABLE dbo.XIImportBatch_T

	
CREATE TABLE [XIImportBatch_T]
(
	[FKiFileID] INT NULL,
	[XICreatedBy] VARCHAR(15) Not NULL,
	[XICreatedWhen] datetime NOT NULL,
	[XIUpdatedBy] VARCHAR(15) Not NULL,
	[XIUpdatedWhen] datetime NOT NULL,
	[XIDeleted] INT NULL,
	[sHierarchy] VARCHAR(256) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[FKiFileIDXIGUID] UNIQUEIDENTIFIER NULL,
	[FKiBODID] INT NULL,
	[FKiBODIDXIGUID] UNIQUEIDENTIFIER NULL,
	[sAttributes] VARCHAR(1024) NULL,
	[bIgnoreFirstLine] bit NULL,
	[sName] VARCHAR(256) NULL,
	[sDescription] VARCHAR(256) NULL,
	[sCode] VARCHAR(32) NULL,
	[iStatus] INT NULL,
	[iType] INT NULL,
	[ID] INT NULL
)