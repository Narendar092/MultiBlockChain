IF OBJECT_ID('OrganizationContacts', 'U') IS NOT NULL DROP TABLE dbo.OrganizationContacts

	
CREATE TABLE [OrganizationContacts]
(
	[CreatedByID] INT NOT NULL default((0)),
	[CreatedByName] VARCHAR(32) NOT NULL default((0)),
	[CreatedBySYSID] VARCHAR(32) NOT NULL default((0)),
	[CreatedTime] datetime NOT NULL default((0)),
	[ModifiedByID] INT NOT NULL default((0)),
	[ModifiedByName] VARCHAR(32) NOT NULL default((0)),
	[ModifiedTime] datetime NOT NULL default((0)),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate()),
	[OrganizationID] INT NOT NULL,
	[Email] VARCHAR(64) Not NULL,
	[Name] VARCHAR(64) Not NULL,
	[Address] VARCHAR(128) Not NULL,
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[Phone] VARCHAR(64) Not NULL
)