CREATE TABLE [dbo].[Signature_T](
	[ID] [int] NULL,
	[XICreatedBy] [varchar](15) NOT NULL,
	[XICreatedWhen] [datetime] NOT NULL,
	[XIUpdatedBy] [varchar](15) NOT NULL,
	[XIUpdatedWhen] [datetime] NOT NULL,
	[XIDeleted] [int] NULL,
	[sHierarchy] [varchar](256) NULL,
	[XIGUID] [uniqueidentifier] NOT NULL,
	[sName] [varchar](256) NULL,
	[sDescription] [varchar](256) NULL,
	[sCode] [varchar](256) NULL,
	[iStatus] [int] NULL,
	[iType] [int] NULL,
	[FKiUserID] [int] NULL,
	[sSignature] [varchar](50) NULL,
	[FKiClassID] [int] NULL,
	[FKiFirstName] [varchar](50) NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Signature_T] ADD  CONSTRAINT [DF__Signature__XIGUI__515009E6]  DEFAULT (newid()) FOR [XIGUID]
GO


