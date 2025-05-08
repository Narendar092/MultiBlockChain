IF OBJECT_ID('WhatsAppUserResponse_T', 'U') IS NOT NULL DROP TABLE dbo.WhatsAppUserResponse_T

	
CREATE TABLE WhatsAppUserResponse_T(
	[ID] [int] NULL,
	[XICreatedBy] [varchar](15) NOT NULL,
	[XICreatedWhen] [datetime] NOT NULL,
	[XIUpdatedBy] [varchar](15) NOT NULL,
	[XIUpdatedWhen] [datetime] NOT NULL,
	[XIDeleted] [int] NULL,
	[sHierarchy] [varchar](256) NULL,
	[XIGUID] [UNIQUEIDENTIFIER] NOT NULL default(newid()),
	[sName] [varchar](256) NULL,
	[sDescription] [varchar](256) NULL,
	[sCode] [varchar](256) NULL,
	[iStatus] [int] NULL,
	[iType] [int] NULL,
	[sContentSID] [varchar](256) NULL,
	[sPhonenumber] [varchar](256) NULL,
	[sMessage] [varchar](256) NULL,
	[FKiQuestionSetID] [varchar](256) NULL
)