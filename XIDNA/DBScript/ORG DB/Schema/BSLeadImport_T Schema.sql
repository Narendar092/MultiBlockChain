IF OBJECT_ID('BSLeadImport_T', 'U') IS NOT NULL DROP TABLE dbo.BSLeadImport_T

	
CREATE TABLE [BSLeadImport_T]
(
	[ID] [int] NULL,
	[XICreatedBy] [varchar](15) NOT NULL,
	[XICreatedWhen] [datetime] NOT NULL,
	[XIUpdatedBy] [varchar](15) NOT NULL,
	[XIUpdatedWhen] [datetime] NOT NULL,
	[XIDeleted] [int] NULL,
	[sHierarchy] [varchar](256) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[sName] [varchar](256) NULL,
	[sDescription] [varchar](256) NULL,
	[sCode] [varchar](256) NULL,
	[iStatus] [int] NULL,
	[iType] [int] NULL,
	[FKiDocumentID] [int] NULL,
	[FKiSourceID] [int] NULL,
	[FKiQuestionSetID] [int] NULL,
	[FKiBODIDXIGUID] [uniqueidentifier] NULL,
	[FKiSubscriptionID] [int] NULL,
	[FKiAppID] [bigint] NULL,
	[FKiOrgID] [bigint] NULL
)