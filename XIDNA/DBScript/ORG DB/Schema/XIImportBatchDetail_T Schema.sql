IF OBJECT_ID('XIImportBatchDetail_T', 'U') IS NOT NULL DROP TABLE dbo.XIImportBatchDetail_T

	
CREATE TABLE [XIImportBatchDetail_T]
(
	[ID] INT NULL,
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
	[FKiBatchID] INT NULL,
	[FKiBatchIDXIGUID] UNIQUEIDENTIFIER NULL,
	[sMessage] VARCHAR(1024) NULL
)