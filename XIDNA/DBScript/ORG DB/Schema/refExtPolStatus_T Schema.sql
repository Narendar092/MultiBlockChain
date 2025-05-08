IF OBJECT_ID('refExtPolStatus_T', 'U') IS NOT NULL DROP TABLE dbo.refExtPolStatus_T

	
CREATE TABLE [refExtPolStatus_T]
(
	[id] INT NOT NULL,
	[sName] NVARCHAR(50) NULL,
	[iStatus] INT NULL,
	[izXDeleted] INT NULL,
	[sNotes] NVARCHAR(250) NULL,
	[ps_polstatus] INT NOT NULL,
	[ps_description] NVARCHAR(20) NULL,
	[ps_mnemonic] NVARCHAR(4) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)