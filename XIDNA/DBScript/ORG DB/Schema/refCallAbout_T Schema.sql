IF OBJECT_ID('refCallAbout_T', 'U') IS NOT NULL DROP TABLE dbo.refCallAbout_T

	
CREATE TABLE [refCallAbout_T]
(
	[id] INT NOT NULL,
	[sName] NVARCHAR(50) NULL,
	[iStatus] INT NULL,
	[izXDeleted] INT NULL,
	[sNotes] NVARCHAR(250) NULL,
	[zXCrtdBy] NVARCHAR(15) NULL,
	[zXCrtdWhn] smalldatetime NULL,
	[zXUpdtdBy] NVARCHAR(15) NULL,
	[zXUpdtdWhn] smalldatetime NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)