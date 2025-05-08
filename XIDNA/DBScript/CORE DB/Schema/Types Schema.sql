IF OBJECT_ID('Types', 'U') IS NOT NULL DROP TABLE dbo.Types

	
CREATE TABLE [Types]
(
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate()),
	[Code] INT NOT NULL default((0)),
	[TypeID] INT NOT NULL default((0)),
	[Name] VARCHAR(32) NOT NULL default(''),
	[Value] INT NOT NULL default((0)),
	[Expression] VARCHAR(32) NOT NULL default(''),
	[Status] INT NOT NULL default((0)),
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[FileName] VARCHAR(256) NULL,
	[Icon] VARCHAR(64) NULL
)