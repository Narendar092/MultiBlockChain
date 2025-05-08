IF OBJECT_ID('MergeQsSummary_T', 'U') IS NOT NULL DROP TABLE dbo.MergeQsSummary_T

	
CREATE TABLE [MergeQsSummary_T]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[sContent] NVARCHAR(MAX) NULL,
	[izXDeleted] INT NULL,
	[zXCrtdBy] VARCHAR(15) NULL,
	[zXCrtdWhn] datetime NULL,
	[zXUpdtdBy] VARCHAR(15) NULL,
	[zXUpdtdWhn] datetime NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)