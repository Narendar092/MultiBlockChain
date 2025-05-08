IF OBJECT_ID('XIDistribute_T', 'U') IS NOT NULL DROP TABLE dbo.XIDistribute_T

	
CREATE TABLE [XIDistribute_T]
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
	[sStatusAttribute] VARCHAR(12) NULL,
	[iSuccessStatus] VARCHAR(12) NULL,
	[iNonAssignedStatus] VARCHAR(12) NULL,
	[FKiBODID] INT NULL,
	[FKiOrgID] INT NULL,
	[FKiBODIDXIGUID] UNIQUEIDENTIFIER NULL,
	[dtReceivedDate] datetime NULL
)