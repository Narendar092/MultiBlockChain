IF OBJECT_ID('BSPPilot_T', 'U') IS NOT NULL DROP TABLE dbo.BSPPilot_T

	
CREATE TABLE [BSPPilot_T]
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
	[FKiTemplateID] [int] NULL,
	[sTemplateID] [varchar](128) NULL,
	[FKiQSDID] [int] NULL,
	[FKiQSDIDXIGUID] [uniqueidentifier] NULL,
	[FKiOrgID] [int] NULL,
	[FKiAppID] [int] NULL
)