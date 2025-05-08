IF OBJECT_ID('BSPCampaign_T', 'U') IS NOT NULL DROP TABLE dbo.BSPCampaign_T

	
CREATE TABLE [BSPCampaign_T]
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
	[FKiPilotID] [int] NULL,
	[FKiLeadImportID] [int] NULL,
	[dtEvent] [datetime] NULL,
	[sWhatsappID] [varchar](128) NULL,
	[tEventTime] [time](7) NULL,
	[sEventTime] [varchar](16) NULL,
	[iThrottleType] [int] NULL,
	[iCount] [int] NULL,
	[iThrottle] [int] NULL,
	[iTimeInterval] [int] NULL,
	[FKiSubscriptionID] [int] NULL,
	[FKiAppID] [bigint] NULL,
	[FKiOrgID] [bigint] NULL,
	[FKiBODIDXIGUID] UNIQUEIDENTIFIER NULL,
	[FKi1QueryXIGUID] UNIQUEIDENTIFIER NULL,
	[FKiWhatsappAccountID] [int] NULL,
	[FKiSMSAccountID] [int] NULL,
	[FKiEmailAccountID] [int] NULL

)