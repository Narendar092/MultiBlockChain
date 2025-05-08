IF OBJECT_ID('PFRequestResponses_T', 'U') IS NOT NULL DROP TABLE dbo.PFRequestResponses_T

	
CREATE TABLE [PFRequestResponses_T]
(
	[ID] INT NOT NULL,
	[sRequest] VARCHAR(MAX) NULL,
	[sResponse] VARCHAR(MAX) NULL,
	[FKiACPolicyID] INT NULL,
	[zXCrtdBy] VARCHAR(120) NULL,
	[zXCrtdWhn] datetime NOT NULL default(getdate()),
	[zXUpdtdBy] VARCHAR(120) NULL,
	[zXUpdtdWhn] datetime NOT NULL default(getdate()),
	[izXDeleted] INT NULL default((0)),
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)