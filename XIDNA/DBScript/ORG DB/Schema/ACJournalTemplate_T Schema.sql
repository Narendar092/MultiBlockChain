IF OBJECT_ID('ACJournalTemplate_T', 'U') IS NOT NULL DROP TABLE dbo.ACJournalTemplate_T

	
CREATE TABLE [ACJournalTemplate_T]
(
	[id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[sName] VARCHAR(255) NULL,
	[iStatus] INT NULL,
	[izXDeleted] INT NULL,
	[sNotes] VARCHAR(255) NULL,
	[iType] INT NULL,
	[FKiOrgID] INT NULL,
	[FKiTransTypeID] INT NULL,
	[FKiCRAccountID] INT NULL,
	[FKiDRAccountID] INT NULL,
	[zXCrtdBy] VARCHAR(255) NULL,
	[zXCrtdWhn] datetime NULL,
	[zXUpdtdBy] VARCHAR(255) NULL,
	[zXUpdtdWhn] datetime NULL,
	[iAmount] INT NULL,
	[FKiAccountID] INT NULL,
	[iReconcilliationType] INT NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)