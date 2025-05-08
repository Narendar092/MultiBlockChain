IF OBJECT_ID('XAttachment_T', 'U') IS  NULL --DROP TABLE dbo.XAttachment_T

	
CREATE TABLE [XAttachment_T]
(
	[ID] INT NULL,
	[XICreatedBy] VARCHAR(15) Not NULL,
	[XICreatedWhen] datetime NOT NULL,
	[XIUpdatedBy] VARCHAR(15) Not NULL,
	[XIUpdatedWhen] datetime NOT NULL,
	[XIDeleted] INT NULL,
	[sHierarchy] VARCHAR(256) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[sName] VARCHAR(256) NULL,
	[sDescription] VARCHAR(256) NULL,
	[sCode] VARCHAR(256) NULL,
	[iStatus] INT NULL,
	[iType] INT NULL,
	[FKiCommunicationID] INT NULL,
	[FKiCommunicationIDXIGUID] UNIQUEIDENTIFIER NULL,
	[FKiDocumentID] INT NULL,
	[FKiDocumentIDXIGUID] UNIQUEIDENTIFIER NULL,
	[FKiPolicyID] INT NULL
)