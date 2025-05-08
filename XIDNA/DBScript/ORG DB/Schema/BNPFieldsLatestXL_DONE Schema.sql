IF OBJECT_ID('BNPFieldsLatestXL_DONE', 'U') IS NOT NULL DROP TABLE dbo.BNPFieldsLatestXL_DONE

	
CREATE TABLE [BNPFieldsLatestXL_DONE]
(
	[id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[FieldName] NVARCHAR(1024) NULL,
	[Type] NVARCHAR(1024) NULL,
	[ServiceName] NVARCHAR(1024) NULL,
	[Mandatory] float NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)