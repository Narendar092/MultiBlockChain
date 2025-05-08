IF OBJECT_ID('ACAccount_T', 'U') IS NOT NULL DROP TABLE dbo.ACAccount_T

	
CREATE TABLE [ACAccount_T]
(
	[id] INT NOT NULL,
	[sName] VARCHAR(50) NULL,
	[iStatus] INT NULL,
	[izXDeleted] INT NULL,
	[sNotes] VARCHAR(250) NULL,
	[sAlert] VARCHAR(50) NULL,
	[iType] INT NULL,
	[FKiOrgID] INT NULL,
	[sAccountCode] VARCHAR(50) NULL,
	[rBalance] float NULL,
	[dStart] datetime NULL,
	[dEnd] datetime NULL,
	[FKiClientID] INT NULL,
	[zXCrtdBy] VARCHAR(15) NULL,
	[zXCrtdWhn] datetime NULL,
	[zXUpdtdBy] VARCHAR(15) NULL,
	[zXUpdtdWhn] datetime NULL,
	[iOverrideType] INT NULL,
	[refControlSectionID] VARCHAR(30) NULL,
	[FKiSysApplicationID] INT NULL,
	[refACGroup] INT NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)