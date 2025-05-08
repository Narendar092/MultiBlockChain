IF OBJECT_ID('refRestricted_T', 'U') IS NOT NULL DROP TABLE dbo.refRestricted_T

	
CREATE TABLE [refRestricted_T]
(
	[id] INT NOT NULL,
	[sName] VARCHAR(50) NULL,
	[zLoad] float NULL,
	[zLoadAge] float NULL,
	[iStatus] INT NULL,
	[izXDeleted] INT NULL,
	[sNotes] VARCHAR(250) NULL,
	[FKiProductID] INT NULL,
	[iVersion] INT NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)