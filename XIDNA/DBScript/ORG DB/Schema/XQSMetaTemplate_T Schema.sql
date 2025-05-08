IF OBJECT_ID('XQSMetaTemplate_T', 'U') IS NOT NULL DROP TABLE dbo.XQSMetaTemplate_T

	
CREATE TABLE [XQSMetaTemplate_T]
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
	[FKiAppID] [int] NULL,
	[FKiOrgID] [int] NULL,
	[FKiQSDIDXIGUID] [uniqueidentifier] NOT NULL,
	[sAttribute] [varchar](64) NULL,
	[sScript] [varchar](max) NULL,
	[sImageID] [varchar](32) NULL,
	[iOrder] decimal(18,4) NULL
)