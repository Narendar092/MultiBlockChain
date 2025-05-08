IF OBJECT_ID('ACrefAccountNo_T', 'U') IS NOT NULL DROP TABLE dbo.ACrefAccountNo_T

	
CREATE TABLE [ACrefAccountNo_T]
(
	[id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[sName] VARCHAR(255) NULL,
	[iStatus] INT NULL,
	[izXDeleted] INT NULL,
	[sNotes] VARCHAR(255) NULL,
	[iType] INT NULL,
	[FKiOrgID] INT NULL,
	[FKiAccountNoID] INT NULL,
	[FKirefConfigurationID] INT NULL,
	[FKiACrefBaseAccountID] INT NULL,
	[zXCrtdBy] VARCHAR(255) NULL,
	[zXCrtdWhn] datetime NULL,
	[zXUpdtdBy] VARCHAR(255) NULL,
	[zXUpdtdWhn] datetime NULL,
	[FKiEnterpriseID] INT NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)