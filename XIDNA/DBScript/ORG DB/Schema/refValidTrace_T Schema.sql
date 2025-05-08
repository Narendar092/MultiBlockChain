IF OBJECT_ID('refValidTrace_T', 'U') IS NOT NULL DROP TABLE dbo.refValidTrace_T

	
CREATE TABLE [refValidTrace_T]
(
	[ID] INT NOT NULL,
	[sUniqueID] VARCHAR(256) Not NULL,
	[sDescription] VARCHAR(512) Not NULL,
	[FKiLeadQualityID] INT NULL,
	[sName] VARCHAR(256) NULL,
	[FkiOutComesID] INT NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[sReplace] VARCHAR(12) NULL,
	[FKiBOID] INT not NULL,
	[FKiAttrID] INT not NULL,
	[izXDeleted] INT NULL,
	[zXCrtdBy] VARCHAR(MAX) NULL,
	[zXCrtdWhn] datetime NULL,
	[zXUpdtdBy] VARCHAR(MAX) NULL,
	[zXUpdtdWhn] datetime NULL,
	[iStatus] bit NOT NULL default((1)),
	[bShowButtons] bit NULL,
	[sVisibleGroup] VARCHAR(32) NULL,
	[sLockGroup] VARCHAR(32) NULL,
	[sSummaryGroup] VARCHAR(32) NULL,
	[FKiOrgID] INT NULL,
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate()),
	[FKiStepDID] INT NULL,
	[FKiStepDIDXIGUID] UNIQUEIDENTIFIER NULL,
	[FKiQSDID] INT NULL,
	[FKiQSDIDXIGUID] UNIQUEIDENTIFIER NULL
)