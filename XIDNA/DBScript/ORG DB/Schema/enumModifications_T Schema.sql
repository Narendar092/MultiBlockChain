IF OBJECT_ID('enumModifications_T', 'U') IS NOT NULL DROP TABLE dbo.enumModifications_T

	
CREATE TABLE [enumModifications_T]
(
	[sName] NVARCHAR(255) NULL,
	[ID] float NULL,
	[izXDeleted] INT NULL,
	[ABICode] VARCHAR(64) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)