IF OBJECT_ID('refExtInsurer_T', 'U') IS NOT NULL DROP TABLE dbo.refExtInsurer_T

	
CREATE TABLE [refExtInsurer_T]
(
	[id] INT NOT NULL,
	[sName] NVARCHAR(50) NULL,
	[iStatus] INT NULL,
	[izXDeleted] INT NULL,
	[sNotes] NVARCHAR(250) NULL,
	[insr_sname] NVARCHAR(20) NULL,
	[insr_scode] NVARCHAR(3) NOT NULL,
	[insr_lname] NVARCHAR(35) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)