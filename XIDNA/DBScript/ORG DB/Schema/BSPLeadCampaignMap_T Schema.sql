IF OBJECT_ID('BSPLeadCampaignMap_T', 'U') IS NOT NULL DROP TABLE dbo.BSPLeadCampaignMap_T

	
CREATE TABLE [BSPLeadCampaignMap_T]
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
	[FKiLeadID] [int] NULL,
	[FKiCampaignID] [int] NULL,
	[dtEvent] [datetime] NULL,
	[iScheduled] [int] NULL,
	[iCampaignStatus] [int] NULL,
	[FKiCommunicationID] [int] NULL,
	[FKiQSIID] [int] NULL,
	[FKiQSIIDXIGUID] [uniqueidentifier] NULL,
	[FKiCurrentStepDID] [int] NULL,
	[FKiCurrentStepDIDXIGUID] [uniqueidentifier] NULL,
	[FKiTraceID] [int] NULL,
	[FKiStageStatusID] [int] NULL,
	[bReminder] [bit] NULL,
	[sMobileNo] [varchar](64) NULL,
	[FKiAppID] [bigint] NULL,
	[FKiOrgID] [bigint] NULL,
	[FKiBOIID] [int] NULL,
	[FKiBODIDXIGUID] [uniqueidentifier] NULL

)