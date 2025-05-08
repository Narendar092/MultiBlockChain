IF OBJECT_ID('XIScheduleInstance_T', 'U') IS NOT NULL DROP TABLE dbo.XIScheduleInstance_T

	
CREATE TABLE [XIScheduleInstance_T]
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
	[FKiQSDID] [int] NULL,
	[FKiQSDIDXIGUID] [uniqueidentifier] NULL,
	[dRunDate] [datetime] NULL,
	[FKiBODID] [int] NULL,
	[FKiBODIDXIGUID] [uniqueidentifier] NULL,
	[FKiBOIID] [int] NULL,
	[FKiQSIID] [int] NULL,
	[FKiQSIIDXIGUID] [uniqueidentifier] NULL,
	[FKiTemplateID] [int] NULL,
	[FKiStepDXIGUID] [uniqueidentifier] NULL,
	[sMobileNo] [varchar](64) NULL,
	[FKiCampaignID] [int] NULL,
	[FKiAppID] [bigint] NULL,
	[FKiOrgID] [bigint] NULL
)