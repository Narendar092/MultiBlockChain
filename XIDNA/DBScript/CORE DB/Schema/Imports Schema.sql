IF OBJECT_ID('Imports', 'U') IS NOT NULL DROP TABLE dbo.Imports

	
CREATE TABLE [Imports]
(
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate()),
	[RuleValue] NVARCHAR(MAX) NULL,
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[RuleType] NVARCHAR(MAX) NULL,
	[Count] INT NOT NULL,
	[RuleName] NVARCHAR(MAX) NULL
)