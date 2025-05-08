CREATE TABLE [dbo].[XIUserDetails_T](
	[ID] [int] NULL,
	[XICreatedBy] [varchar](15) NOT NULL,
	[XICreatedWhen] [datetime] NOT NULL,
	[XIUpdatedBy] [varchar](15) NOT NULL,
	[XIUpdatedWhen] [datetime] NOT NULL,
	[XIDeleted] [int] NULL,
	[sHierarchy] [varchar](256) NULL,
	[XIGUID] [uniqueidentifier] NOT NULL,
	[FKiFirstName] [varchar](50) NULL,
	[FKiLastName] [varchar](50) NULL,
	[FKiEmail] [varchar](50) NULL,
	[FKiUserID] [int] NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[XIUserDetails_T] ADD  DEFAULT (newid()) FOR [XIGUID]
GO


