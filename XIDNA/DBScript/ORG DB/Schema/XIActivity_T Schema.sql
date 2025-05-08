IF OBJECT_ID('XIActivity_T', 'U') IS NOT NULL DROP TABLE dbo.XIActivity_T

	
CREATE TABLE [dbo].[XIActivity_T](
	[ID] [int] NULL,
	[XICreatedBy] [varchar](15) NOT NULL,
	[XICreatedWhen] [datetime] NOT NULL,
	[XIUpdatedBy] [varchar](15) NOT NULL,
	[XIUpdatedWhen] [datetime] NOT NULL,
	[XIDeleted] [int] NULL,
	[sHierarchy] [varchar](256) NULL,
	[XIGUID] [uniqueidentifier] not null default(newid()),
	[sName] [varchar](256) NULL,
	[sDescription] [varchar](256) NULL,
	[sCode] [varchar](256) NULL,
	[iStatus] [int] NULL,
	[iType] [int] NULL,
	[FKiBODID] [int] NULL,
	[FKiBODIDXIGUID] [uniqueidentifier] NULL,
	[FKiBOIID] [int] NULL,
	[FKsCategoryID] [varchar](32) NULL,
	[iCriticality] [int] NULL,
	[refCategoryID] [int] NULL
)