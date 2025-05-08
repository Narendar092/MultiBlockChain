IF OBJECT_ID('Targets', 'U') IS NOT NULL DROP TABLE dbo.Targets

	
CREATE TABLE [Targets]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[OrganizationID] INT NULL,
	[ReportID] INT NULL,
	[UserID] INT NULL,
	[ColumnID] INT NULL,
	[Target] INT NULL,
	[Period] VARCHAR(32) NULL,
	[Colour] VARCHAR(32) NULL,
	[IsSMS] bit NULL,
	[IsEmail] bit NULL,
	[IsNotification] bit NULL,
	[SMSTemplateID] INT NULL,
	[EmailTemplateID] INT NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[FKi1ClickIDXIGUID] UNIQUEIDENTIFIER NULL
)