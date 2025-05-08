IF OBJECT_ID('DocumentType', 'U') IS NOT NULL DROP TABLE dbo.DocumentType

	
CREATE TABLE [DocumentType]
(
	[sTypeName] NVARCHAR(200) NOT NULL,
	[sNotes] VARCHAR(250) NULL,
	[izxDeleted] INT NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)