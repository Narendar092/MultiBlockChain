IF OBJECT_ID('KpiDetails', 'U') IS NOT NULL DROP TABLE dbo.KpiDetails

	
CREATE TABLE [KpiDetails]
(
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate()),
	[ReportID] INT NULL,
	[IsSystemGenerated] bit NULL,
	[CreatedDate] datetime NULL,
	[Text] NVARCHAR(MAX) NULL,
	[KpiDetailsId] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[KpiName] NVARCHAR(150) NULL,
	[IsSubscribed] bit NULL,
	[OrganizationId] INT NULL,
	[Type] VARCHAR(32) NULL,
	[Description] NVARCHAR(MAX) NULL,
	[KpiDetailsSubId] INT NULL,
	[ColorCode] NVARCHAR(150) NULL,
	[OrganizationLevel] INT NULL,
	[CreatedBy] INT NULL,
	[UserID] INT NULL,
	[Threshold] INT NULL,
	[PostColorCode] NVARCHAR(MAX) NULL,
	[NotificationType] NVARCHAR(150) NULL,
	[UpdatedDate] datetime NULL,
	[ThresholdNumerator] INT NULL,
	[PreColorCode] NVARCHAR(MAX) NULL,
	[Rank] INT NULL,
	[DisplayAsID] INT NULL
)