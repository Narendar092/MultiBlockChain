IF OBJECT_ID('RiskFactor_T', 'U') IS NOT NULL DROP TABLE dbo.RiskFactor_T

	
CREATE TABLE [RiskFactor_T]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[FKiQuoteID] bigint NULL,
	[FKiPolicyID] bigint NULL,
	[sFactorName] VARCHAR(128) NULL,
	[sValue] VARCHAR(32) NULL,
	[sMessage] VARCHAR(256) NULL,
	[CreatedTime] datetime NULL,
	[FKsQuoteID] VARCHAR(128) NULL,
	[izXDeleted] INT NOT NULL default((0)),
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[FKiOrgID] INT NULL,
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)