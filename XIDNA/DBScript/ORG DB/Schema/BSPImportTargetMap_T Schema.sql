IF OBJECT_ID('BSPImportTargetMap_T', 'U') IS NOT NULL DROP TABLE dbo.BSPImportTargetMap_T

	
CREATE TABLE [BSPImportTargetMap_T]
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
	[FKiAppID] [bigint] NULL,
	[FKiOrgID] [bigint] NULL,
	[FKiTargetID] [int] NULL,
	[FKiImportID] [int] NULL,
	[FKiBODID] [int] NULL,
	[FKiBOIID] [int] NULL,
	[FKiBODIDXIGUID] [uniqueidentifier] NULL
)