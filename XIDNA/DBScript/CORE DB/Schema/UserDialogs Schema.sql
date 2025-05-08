IF OBJECT_ID('UserDialogs', 'U') IS NOT NULL DROP TABLE dbo.UserDialogs

	
CREATE TABLE [UserDialogs]
(
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate()),
	[OneClickID] INT NULL,
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[OrganizationID] INT NULL,
	[Status] bit NULL,
	[UserID] INT NULL
)