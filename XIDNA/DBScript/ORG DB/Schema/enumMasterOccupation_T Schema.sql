IF OBJECT_ID('enumMasterOccupation_T', 'U') IS NOT NULL DROP TABLE dbo.enumMasterOccupation_T

	
CREATE TABLE [enumMasterOccupation_T]
(
	[ID] INT NOT NULL,
	[sName] NVARCHAR(255) NULL,
	[sValue] VARCHAR(255) NULL,
	[iStatus] INT NULL,
	[izXDeleted] INT NULL,
	[sNotes] VARCHAR(250) NULL,
	[iType] INT NULL,
	[FKiOrgID] INT NULL,
	[zXCrtdBy] VARCHAR(15) NULL,
	[zXCrtdWhn] datetime NULL,
	[zXUpdtdBy] VARCHAR(15) NULL,
	[zXUpdtdWhn] datetime NULL,
	[FKiImportID] INT NULL,
	[iImportStatus] INT NULL,
	[bIsDecline] bit NULL,
	[iPremiumLoad] INT NULL,
	[FkiClassID] INT NULL,
	[sTerm] VARCHAR(MAX) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)