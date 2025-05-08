IF OBJECT_ID('BNPFieldsLatestXL', 'U') IS NOT NULL DROP TABLE dbo.BNPFieldsLatestXL

	
CREATE TABLE [BNPFieldsLatestXL]
(
	[id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[sFieldName] NVARCHAR(1024) NULL,
	[sType] NVARCHAR(1024) NULL,
	[sServiceName] NVARCHAR(1024) NULL,
	[bMandatory] float NULL,
	[iMinLength] INT NULL,
	[iMaxLength] INT NULL,
	[sRegexp] VARCHAR(256) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)