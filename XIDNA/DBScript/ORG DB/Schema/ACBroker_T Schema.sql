IF OBJECT_ID('ACBroker_T', 'U') IS NOT NULL DROP TABLE dbo.ACBroker_T

	
CREATE TABLE [ACBroker_T]
(
	[id] INT NOT NULL,
	[sName] VARCHAR(50) NULL,
	[iStatus] INT NULL,
	[izXDeleted] INT NULL,
	[sNotes] VARCHAR(250) NULL,
	[sAlert] VARCHAR(50) NULL,
	[iType] INT NULL,
	[FKiOrgID] INT NULL,
	[FKiACAccountID] INT NULL,
	[sEMail] VARCHAR(250) NULL,
	[sFullAddress] VARCHAR(250) NULL,
	[sPostCode] VARCHAR(10) NULL,
	[FKiSysApplicationID] INT NULL,
	[zXCrtdBy] VARCHAR(15) NULL,
	[zXCrtdWhn] datetime NULL,
	[zXUpdtdBy] VARCHAR(15) NULL,
	[zXUpdtdWhn] datetime NULL,
	[sReference] VARCHAR(50) NULL,
	[sExternalRef] VARCHAR(50) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)