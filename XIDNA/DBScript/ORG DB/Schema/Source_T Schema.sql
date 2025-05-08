IF OBJECT_ID('Source_T', 'U') IS NOT NULL DROP TABLE dbo.Source_T

	
CREATE TABLE [Source_T]
(
	[ID] INT NOT NULL,
	[sName] VARCHAR(64) NULL,
	[sDescription] VARCHAR(256) NULL,
	[iStatus] INT NULL,
	[sCode] VARCHAR(32) NULL,
	[sPrefixCode] VARCHAR(32) NULL,
	[refAccountCategory] INT NULL,
	[izXDeleted] INT NULL,
	[zXCrtdBy] VARCHAR(32) NULL,
	[zXCrtdWhn] datetime NULL,
	[zXUpdtdBy] VARCHAR(32) NULL,
	[zXUpdtdWhn] datetime NULL,
	[FKiOriginID] INT not NULL,
	[FKiApplicationID] INT not NULL,
	[OrganisationID] INT NOT NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)