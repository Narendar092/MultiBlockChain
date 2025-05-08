IF OBJECT_ID('XWhatsappObject_T', 'U') IS NOT NULL DROP TABLE dbo.XWhatsappObject_T

	
CREATE TABLE [XWhatsappObject_T]
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
	[sMob] [varchar](128) NULL,
	[sContentSID] [varchar](128) NULL,
	[FKiBODID] [int] NULL,
	[FKiBOIID] [int] NULL,
	[FKiCommunicationID] [int] NULL,
	[FKiPilotID] [int] NULL,
	[FKiPilotIDXIGUID] [uniqueidentifier] NULL,
	[FKiCampaignID] [int] NULL,
	[FKiCampaignIDXIGUID] [uniqueidentifier] NULL,
	[FKiQSIIDXIGUID] [uniqueidentifier] NULL,
	[FKiAppID] [bigint] NULL,
	[FKiOrgID] [bigint] NULL
)