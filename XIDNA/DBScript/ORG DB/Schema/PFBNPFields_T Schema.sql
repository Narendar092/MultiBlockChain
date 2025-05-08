IF OBJECT_ID('PFBNPFields_T', 'U') IS NOT NULL DROP TABLE dbo.PFBNPFields_T

	
CREATE TABLE [PFBNPFields_T]
(
	[id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[sIPMFieldName] VARCHAR(256) NULL,
	[sXIFieldName] VARCHAR(256) NULL,
	[sfDisplayName] VARCHAR(256) NULL,
	[sType] VARCHAR(256) NULL,
	[sNotes] VARCHAR(1024) NULL,
	[sServiceName] VARCHAR(256) NULL,
	[bIsIPMMandatory] bit NULL,
	[bIsMandatory] bit NOT NULL,
	[iMinLength] INT NOT NULL,
	[iMaxLength] INT NOT NULL,
	[sRegexp] VARCHAR(128) NULL,
	[sErrorMessage] VARCHAR(1024) NULL,
	[sNotation] VARCHAR(256) NULL,
	[sDefaultValue] VARCHAR(128) NULL,
	[sFormat] VARCHAR(32) NULL,
	[sIPMNotation] VARCHAR(256) NULL,
	[sIPMDefaultValue] VARCHAR(128) NULL,
	[sIPMObjectEvaluation] VARCHAR(MAX) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)