IF OBJECT_ID('refCountry_T', 'U') IS NOT NULL DROP TABLE dbo.refCountry_T

	
CREATE TABLE [refCountry_T]
(
	[id] INT NOT NULL,
	[sName] VARCHAR(50) NULL,
	[zLoadPC] float NULL,
	[rXS] float NULL,
	[iStatus] INT NULL,
	[izXDeleted] INT NULL,
	[sNotes] VARCHAR(250) NULL,
	[sCountries] text NULL,
	[FKiProductID] INT NULL,
	[iVersion] INT NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)