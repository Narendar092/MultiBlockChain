IF OBJECT_ID('RefTraceStage_T', 'U') IS NOT NULL DROP TABLE dbo.RefTraceStage_T

	
CREATE TABLE [RefTraceStage_T]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[sName] VARCHAR(250) Not NULL,
	[sDescription] VARCHAR(250) Not NULL,
	[iStatus] INT NOT NULL,
	[izXDeleted] INT NULL default((0)),
	[zXCrtdBy] VARCHAR(15) NULL,
	[zXCrtdWhn] datetime NULL,
	[zXUpdtdBy] VARCHAR(15) NULL,
	[zXUpdtdWhn] datetime NULL,
	[sStatusName1] VARCHAR(64) NULL,
	[iStatusValue1] VARCHAR(64) NULL,
	[sStatusName2] VARCHAR(64) NULL,
	[iStatusValue2] VARCHAR(64) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[FKiBOID] INT not NULL,
	[FKiAttrID] INT not NULL,
	[FKiXILinkID] INT NULL,
	[FKiOrgID] INT NULL,
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate()),
	[FKiBOIDXIGUID] UNIQUEIDENTIFIER NULL,
	[FKiAttrIDXIGUID] UNIQUEIDENTIFIER NULL,
	[FKiXILinkIDXIGUID] UNIQUEIDENTIFIER NULL,
	[iTraceTrigger] INT NULL,
	[FKiStepDID] INT NULL,
	[FKiStepDIDXIGUID] UNIQUEIDENTIFIER NULL,
	[FKiQSDID] INT NULL,
	[FKiQSDIDXIGUID] UNIQUEIDENTIFIER NULL,
	[iType] INT NULL
)