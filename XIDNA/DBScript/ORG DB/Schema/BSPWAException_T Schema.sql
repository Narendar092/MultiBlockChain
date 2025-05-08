IF OBJECT_ID('BSPWAException_T', 'U') IS NOT NULL DROP TABLE dbo.BSPWAException_T

	
CREATE TABLE [BSPWAException_T]
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
	[XTargetCampaignID] [int] NULL,
	[sMobileNo] [varchar](64) NULL,
	[sMessage] [varchar](max) NULL,
	[FKiXWhatsappObjID] [int] NULL,
	[FKiAppID] [bigint] NULL,
	[FKiOrgID] [bigint] NULL

)