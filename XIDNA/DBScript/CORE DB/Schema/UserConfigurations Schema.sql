IF OBJECT_ID('UserConfigurations', 'U') IS NOT NULL DROP TABLE dbo.UserConfigurations

	
CREATE TABLE [UserConfigurations]
(
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate()),
	[UpdatedBy] INT NOT NULL,
	[Name] VARCHAR(32) Not NULL,
	[CreatedBy] INT NOT NULL,
	[UpdatedTime] datetime NOT NULL,
	[StatusTypeID] INT NOT NULL,
	[CreatedTime] datetime NOT NULL,
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1)
)