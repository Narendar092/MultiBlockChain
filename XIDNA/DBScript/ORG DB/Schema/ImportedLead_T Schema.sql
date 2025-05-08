IF OBJECT_ID('ImportedLead_T', 'U') IS NOT NULL DROP TABLE dbo.ImportedLead_T

	
CREATE TABLE [ImportedLead_T]
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
	[FKiLeadImportID] [int] NULL,
	[FKiSourceID] [int] NULL,
	[sFirstName] [varchar](256) NULL,
	[sLastName] [varchar](256) NULL,
	[sMob] [varchar](256) NULL,
	[sPostCode] [varchar](64) NULL,
	[sEmail] [varchar](256) NULL
)