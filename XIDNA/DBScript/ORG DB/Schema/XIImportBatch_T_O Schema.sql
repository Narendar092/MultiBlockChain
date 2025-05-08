IF OBJECT_ID('XIImportBatch_T_O', 'U') IS NOT NULL DROP TABLE dbo.XIImportBatch_T_O

	
CREATE TABLE [XIImportBatch_T_O]
(
	[ID] INT NOT NULL,
	[XICreatedBy] VARCHAR(15) Not NULL,
	[XICreatedWhen] datetime NOT NULL,
	[XIUpdatedBy] VARCHAR(15) Not NULL,
	[XIUpdatedWhen] datetime NOT NULL,
	[XIDeleted] INT NULL,
	[sHierarchy] VARCHAR(256) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[sName] VARCHAR(256) NULL,
	[sDescription] VARCHAR(256) NULL,
	[sCode] VARCHAR(256) NULL,
	[iStatus] INT NULL,
	[iType] INT NULL,
	[FKiFileID] INT NULL,
	[FKiFileIDXIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[FKiBODID] INT NULL,
	[FKiBODIDXIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[sAttributes] VARCHAR(1024) NULL,
	[bIgnoreFirstLine] bit NULL
)