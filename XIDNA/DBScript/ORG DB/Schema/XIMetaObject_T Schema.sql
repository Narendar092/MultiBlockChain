IF OBJECT_ID('XIMetaObject_T', 'U') IS NOT NULL DROP TABLE dbo.XIMetaObject_T

	
CREATE TABLE [XIMetaObject_T]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
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
	[sMobileNo] [varchar](32) NULL,
	[sObject] [varchar](max) NULL,
	[sReply] [varchar](max) NULL,
	[sStatus] [varchar](64) NULL,
	[FKiQSIIDXIGUID] [uniqueidentifier] NULL,
	[FKiMetaFieldInstanceID] [int] NULL,
	[FKiAppID] [bigint] NULL,
	[FKiOrgID] [bigint] NULL

)