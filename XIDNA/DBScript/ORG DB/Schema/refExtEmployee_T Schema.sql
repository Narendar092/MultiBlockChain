IF OBJECT_ID('refExtEmployee_T', 'U') IS NOT NULL DROP TABLE dbo.refExtEmployee_T

	
CREATE TABLE [refExtEmployee_T]
(
	[id] INT NOT NULL,
	[ecode] NVARCHAR(3) NOT NULL,
	[esurname] NVARCHAR(35) NULL,
	[efname] NVARCHAR(15) NULL,
	[sName] NVARCHAR(50) NULL,
	[iStatus] INT NULL,
	[izXDeleted] INT NULL,
	[sNotes] NVARCHAR(250) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)