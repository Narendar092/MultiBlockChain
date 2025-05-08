IF OBJECT_ID('RequirementChaser_T', 'U') IS NOT NULL DROP TABLE dbo.RequirementChaser_T

	
CREATE TABLE [RequirementChaser_T]
(
	[id] INT NOT NULL,
	[sName] VARCHAR(50) NULL,
	[iStatus] INT NULL,
	[izXDeleted] INT NULL,
	[sNotes] VARCHAR(250) NULL,
	[iType] INT NULL,
	[FKiOrgID] INT NULL,
	[dDue] datetime NULL,
	[iPhoneCall] INT NULL,
	[bSendSMS] bit NULL,
	[bSendEmail] bit NULL,
	[bSendLetter] bit NULL,
	[FKiACPolicyID] INT NULL,
	[FKiClientID] INT NULL,
	[FKiSupplierID] INT NULL,
	[zXCrtdBy] VARCHAR(15) NULL,
	[zXCrtdWhn] datetime NULL,
	[zXUpdtdBy] VARCHAR(15) NULL,
	[zXUpdtdWhn] datetime NULL,
	[sEMail] VARCHAR(120) NULL,
	[sFullChaseMessage] text NULL,
	[sChaserMessage] VARCHAR(4000) NULL,
	[XIGUID] UNIQUEIDENTIFIER not null default(newid()),
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate())
)