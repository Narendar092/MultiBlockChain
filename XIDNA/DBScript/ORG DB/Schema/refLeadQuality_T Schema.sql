IF OBJECT_ID('refLeadQuality_T', 'U') IS NOT NULL DROP TABLE dbo.refLeadQuality_T

	
CREATE TABLE [refLeadQuality_T]
(
	[ID] INT NOT NULL,
	[sLeadQuality] VARCHAR(256) Not NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[FKiBOID] INT not NULL,
	[FKiAttrID] INT not NULL,
	[izXDeleted] INT NULL,
	[zXCrtdBy] VARCHAR(MAX) NULL,
	[zXCrtdWhn] datetime NULL,
	[zXUpdtdBy] VARCHAR(MAX) NULL,
	[zXUpdtdWhn] datetime NULL,
	[FKiOrgID] INT NULL,
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)