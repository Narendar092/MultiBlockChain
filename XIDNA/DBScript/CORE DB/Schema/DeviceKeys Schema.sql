IF OBJECT_ID('DeviceKeys', 'U') IS NOT NULL DROP TABLE dbo.DeviceKeys

	
CREATE TABLE [DeviceKeys]
(
	[XIDeleted] INT NOT NULL default((0)),
	[XICreatedBy] VARCHAR(64) NOT NULL default(''),
	[XIUpdatedBy] VARCHAR(64) NOT NULL default(''),
	[XICreatedWhen] datetime NOT NULL default(getdate()),
	[XIUpdatedWhen] datetime NOT NULL default(getdate()),
	[DeviceKeysId] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[LoginId] INT NULL,
	[KeyDevice] NVARCHAR(MAX) NULL,
	[DeviceKeyType] NVARCHAR(150) NULL
)