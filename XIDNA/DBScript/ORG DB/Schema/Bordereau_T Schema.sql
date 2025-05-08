IF OBJECT_ID('Bordereau_T', 'U') IS NOT NULL DROP TABLE dbo.Bordereau_T

	
CREATE TABLE [Bordereau_T]
(
	[ID] INT NOT NULL,
	[sBordereauFileName] VARCHAR(50) NULL,
	[iStatus] INT NULL,
	[zXCrtdBy] VARCHAR(120) NULL,
	[zXCrtdWhn] datetime NOT NULL default(getdate()),
	[zXUpdtdBy] VARCHAR(120) NULL,
	[zXUpdtdWhn] datetime NOT NULL default(getdate()),
	[izXDeleted] INT NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)