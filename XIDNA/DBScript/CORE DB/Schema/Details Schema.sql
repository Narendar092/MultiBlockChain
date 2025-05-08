IF OBJECT_ID('Details', 'U') IS NOT NULL DROP TABLE dbo.Details

	
CREATE TABLE [Details]
(
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate()),
	[UserID] INT NOT NULL,
	[ParentID] INT NOT NULL,
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[StatusTypeID] INT NOT NULL
)