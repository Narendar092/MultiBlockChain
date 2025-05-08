IF OBJECT_ID('XIBatch_T', 'U') IS NOT NULL DROP TABLE dbo.XIBatch_T

	
CREATE TABLE [XIBatch_T]
(
	[sName] VARCHAR(256) NULL,
	[XICreatedBy] VARCHAR(15) Not NULL,
	[XICreatedWhen] datetime NOT NULL,
	[XIUpdatedBy] VARCHAR(15) Not NULL,
	[XIUpdatedWhen] datetime NOT NULL,
	[XIDeleted] INT NULL,
	[sHierarchy] VARCHAR(256) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[iMob] INT NULL,
	[ID] INT NULL,
	[sDescription] VARCHAR(256) NULL,
	[sCode] VARCHAR(256) NULL,
	[iStatus] INT NULL,
	[iType] INT NULL,
	[iTittle] INT NULL
)