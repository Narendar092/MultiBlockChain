IF OBJECT_ID('OrganizationLocations', 'U') IS NOT NULL DROP TABLE dbo.OrganizationLocations

	
CREATE TABLE [OrganizationLocations]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[OrganizationID] INT NOT NULL,
	[Location] VARCHAR(50) Not NULL,
	[StatusTypeID] INT NOT NULL default((0)),
	[CreatedByID] INT NULL default((0)),
	[CreatedByName] VARCHAR(32) NULL default(''),
	[CreatedBySYSID] VARCHAR(32) NULL default(''),
	[CreatedTime] datetime NULL default('1900-01-01'),
	[ModifiedByID] INT NULL default((0)),
	[ModifiedByName] VARCHAR(32) NULL default(''),
	[ModifiedBySYSID] VARCHAR(32) NULL default(''),
	[ModifiedTime] datetime NULL default('1900-01-01'),
	[LocationCode] VARCHAR(32) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)