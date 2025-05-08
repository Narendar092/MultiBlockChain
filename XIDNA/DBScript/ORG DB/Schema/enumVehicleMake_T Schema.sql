IF OBJECT_ID('enumVehicleMake_T', 'U') IS NOT NULL DROP TABLE dbo.enumVehicleMake_T

	
CREATE TABLE [enumVehicleMake_T]
(
	[id] INT NOT NULL,
	[sName] VARCHAR(100) NULL,
	[iStatus] INT NULL,
	[izXDeleted] INT NULL,
	[sNotes] VARCHAR(250) NULL,
	[iType] INT NULL,
	[FKiOrgID] INT NULL,
	[sValue] VARCHAR(50) NULL,
	[zXCrtdBy] VARCHAR(15) NULL,
	[zXCrtdWhn] datetime NULL,
	[zXUpdtdBy] VARCHAR(15) NULL,
	[zXUpdtdWhn] datetime NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[FKiClassID] INT NULL,
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)