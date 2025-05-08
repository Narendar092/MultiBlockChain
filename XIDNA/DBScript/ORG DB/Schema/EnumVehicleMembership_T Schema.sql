IF OBJECT_ID('EnumVehicleMembership_T', 'U') IS NOT NULL DROP TABLE dbo.EnumVehicleMembership_T

	
CREATE TABLE [EnumVehicleMembership_T]
(
	[ID] INT NOT NULL,
	[sName] VARCHAR(256) NULL,
	[sValue] VARCHAR(256) NULL,
	[iStatus] INT NULL,
	[zXCrtdBy] VARCHAR(15) NULL,
	[zXCrtdWhn] datetime NULL,
	[zXUpdtdBy] VARCHAR(15) NULL,
	[zXUpdtdWhn] datetime NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[izXDeleted] INT NULL default((0)),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)