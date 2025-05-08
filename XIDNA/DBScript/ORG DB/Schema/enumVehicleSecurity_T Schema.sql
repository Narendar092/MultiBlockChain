IF OBJECT_ID('enumVehicleSecurity_T', 'U') IS NOT NULL DROP TABLE dbo.enumVehicleSecurity_T

	
CREATE TABLE [enumVehicleSecurity_T]
(
	[id] float NULL,
	[sName] NVARCHAR(255) NULL,
	[iStatus] NVARCHAR(255) NULL,
	[izXDeleted] float NULL,
	[sNotes] NVARCHAR(255) NULL,
	[iType] NVARCHAR(255) NULL,
	[FKiOrgID] NVARCHAR(255) NULL,
	[sValue] float NULL,
	[FKiImportID] NVARCHAR(255) NULL,
	[zXCrtdBy] NVARCHAR(255) NULL,
	[zXCrtdWhn] NVARCHAR(255) NULL,
	[zXUpdtdBy] NVARCHAR(255) NULL,
	[zXUpdtdWhn] NVARCHAR(255) NULL,
	[sGroupName] VARCHAR(64) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)