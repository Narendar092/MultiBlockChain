IF OBJECT_ID('XIPerformanceCache_T', 'U') IS NOT NULL DROP TABLE dbo.XIPerformanceCache_T

	
CREATE TABLE [XIPerformanceCache_T]
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
	[iCalls] [int] NULL,
	[iMinMS] [float] NULL,
	[iMaxMS] [float] NULL,
	[iAverage] [float] NULL,
	[FKiDefIDXIGUID] [uniqueidentifier] NULL,
	[dDate] [date] NULL
)