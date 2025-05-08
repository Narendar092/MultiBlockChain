IF OBJECT_ID('enumVehicleModel_T', 'U') IS NOT NULL DROP TABLE dbo.enumVehicleModel_T

	
CREATE TABLE [enumVehicleModel_T]
(
	[ID] INT NOT NULL,
	[sName] VARCHAR(256) NULL,
	[sValue] VARCHAR(256) NULL,
	[iStatus] INT NULL,
	[sNotes] VARCHAR(256) NULL,
	[iType] INT NULL,
	[FKiClassID] INT NULL,
	[izXDeleted] INT NULL default((0)),
	[FKiOrgID] INT NULL,
	[zXCrtdBy] VARCHAR(15) NULL,
	[zXCrtdWhn] datetime NULL,
	[zXUpdtdBy] VARCHAR(15) NULL,
	[zXUpdtdWhn] datetime NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)