IF OBJECT_ID('BNPFieldRegexp', 'U') IS NOT NULL DROP TABLE dbo.BNPFieldRegexp

	
CREATE TABLE [BNPFieldRegexp]
(
	[sFieldName] NVARCHAR(255) NULL,
	[iMinLength] float NULL,
	[iMaxLength] float NULL,
	[sRegexp] NVARCHAR(255) NULL,
	[sDocument] NVARCHAR(MAX) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)