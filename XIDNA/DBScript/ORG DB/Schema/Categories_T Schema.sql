IF OBJECT_ID('Categories_T', 'U') IS NOT NULL DROP TABLE dbo.Categories_T

	
CREATE TABLE [Categories_T]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[bIsChangeOfAddress] bit NULL,
	[bIsChangeOfVehicle] bit NULL,
	[bIsAddADriver] bit NULL,
	[bIsChangeOfContactDetails] bit NULL,
	[bIsChangeOfName] bit NULL,
	[zXCrtdBy] VARCHAR(15) NULL,
	[zXCrtdWhn] datetime NULL,
	[zXUpdtdBy] VARCHAR(15) NULL,
	[zXUpdtdWhn] datetime NULL,
	[izXDeleted] INT NULL,
	[sName] VARCHAR(60) NULL,
	[iGroupID] INT NULL,
	[FKiQSDefinitionID] INT NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)