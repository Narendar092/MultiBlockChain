IF OBJECT_ID('refDemeritConvertor_T', 'U') IS NOT NULL DROP TABLE dbo.refDemeritConvertor_T

	
CREATE TABLE [refDemeritConvertor_T]
(
	[id] INT NOT NULL,
	[zLoadPC] float NULL,
	[iPoints] INT NULL,
	[sName] VARCHAR(50) NULL,
	[iStatus] INT NULL,
	[izXDeleted] INT NULL,
	[sNotes] VARCHAR(250) NULL,
	[FKiProductID] INT NULL,
	[iAppliedTo] INT NULL,
	[sConversionFunction] VARCHAR(250) NULL,
	[iVersion] INT NULL,
	[fXSAdditional] float NULL,
	[sXSAddFunc] VARCHAR(4000) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)