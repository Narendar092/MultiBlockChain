IF OBJECT_ID('refCol_T', 'U') IS NOT NULL DROP TABLE dbo.refCol_T

	
CREATE TABLE [refCol_T]
(
	[id] INT NOT NULL,
	[sName] NVARCHAR(50) NULL,
	[iColNo] INT NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)