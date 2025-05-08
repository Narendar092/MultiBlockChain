IF OBJECT_ID('refQuoteTraceStage_T', 'U') IS NOT NULL DROP TABLE dbo.refQuoteTraceStage_T

	
CREATE TABLE [refQuoteTraceStage_T]
(
	[id] INT NOT NULL,
	[sName] VARCHAR(50) NULL,
	[iStatus] INT NULL,
	[izXDeleted] INT NULL,
	[sNotes] VARCHAR(250) NULL,
	[iHotStatusFrom] INT NULL,
	[iHotStatusTo] INT NULL,
	[iTSNo] INT NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)