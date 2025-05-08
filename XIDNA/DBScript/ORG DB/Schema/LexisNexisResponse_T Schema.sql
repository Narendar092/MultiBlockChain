IF OBJECT_ID('LexisNexisResponse_T', 'U') IS NOT NULL DROP TABLE dbo.LexisNexisResponse_T

	
CREATE TABLE [LexisNexisResponse_T]
(
	[id] INT NOT NULL,
	[sRequest] NVARCHAR(MAX) NULL,
	[sResponse] NVARCHAR(MAX) NULL,
	[sResultText] VARCHAR(50) NULL,
	[iSmartscore] INT NULL,
	[FKiQSInstanceID] INT NULL,
	[zXCrtdBy] VARCHAR(15) NULL,
	[zXCrtdWhn] datetime NULL,
	[zXUpdtdBy] VARCHAR(15) NULL,
	[zXUpdtdWhn] datetime NULL,
	[sProfileUrl] VARCHAR(256) NULL,
	[izXDeleted] INT NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)