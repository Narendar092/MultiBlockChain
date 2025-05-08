IF OBJECT_ID('ImportRules', 'U') IS NOT NULL DROP TABLE dbo.ImportRules

	
CREATE TABLE [ImportRules]
(
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate()),
	[Count] INT NOT NULL,
	[RuleName] VARCHAR(64) NULL,
	[RuleValue] VARCHAR(32) NULL,
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[RuleType] VARCHAR(32) NULL
)