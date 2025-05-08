IF OBJECT_ID('XIWhatsappAccount_T', 'U') IS NOT NULL DROP TABLE dbo.XIWhatsappAccount_T

	
CREATE TABLE [XIWhatsappAccount_T]
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
	[sPhoneNumber] [varchar](64) NULL,
	[FKiAppID] [int] NULL,
	[FKiOrgID] [int] NULL
)