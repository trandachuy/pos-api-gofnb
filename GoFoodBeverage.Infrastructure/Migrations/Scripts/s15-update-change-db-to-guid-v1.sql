--Table: Langguage, Country, City, District, Ward, State, Currency, AccountTransfer, UserActivity, Channel
GO
--Table: Langguage > LanguageStore
ALTER TABLE [dbo].[Language]
ADD IdTemp UNIQUEIDENTIFIER NOT NULL DEFAULT(NEWID());
GO

ALTER TABLE [dbo].[LanguageStore]
DROP CONSTRAINT FK_LanguageStore_Language_LanguageId;
GO
ALTER TABLE [dbo].[LanguageStore]
ADD LanguageIdTemp UNIQUEIDENTIFIER
GO

UPDATE b
SET b.[LanguageIdTemp] = a.[IdTemp]
FROM 
[dbo].[LanguageStore] b
INNER JOIN [dbo].[Language] a ON b.[LanguageId] = a.[Id]
GO

IF EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_LanguageStore_LanguageId'
    AND object_id = OBJECT_ID('[dbo].[LanguageStore]'))
BEGIN
    DROP INDEX [IX_LanguageStore_LanguageId] ON [dbo].[LanguageStore]
END
GO
ALTER TABLE [dbo].[LanguageStore]
DROP COLUMN [LanguageId]
GO

EXEC SP_RENAME '[dbo].[LanguageStore].[LanguageIdTemp]', 'LanguageId', 'COLUMN'
GO

ALTER TABLE [dbo].[Language]
DROP CONSTRAINT [PK_Language]
GO

ALTER TABLE [dbo].[Language]
DROP COLUMN [Id]
GO

EXEC SP_RENAME '[dbo].[Language].[IdTemp]', 'Id', 'COLUMN'
GO

ALTER TABLE [dbo].[Language]
ALTER COLUMN [Id] UNIQUEIDENTIFIER not null
GO

ALTER TABLE [dbo].[Language]
ADD CONSTRAINT PK_Language PRIMARY KEY ([Id]);
GO

ALTER TABLE [dbo].[LanguageStore]
ADD CONSTRAINT FK_LanguageStore_Language_LanguageId
FOREIGN KEY ([LanguageId]) REFERENCES [dbo].[Language]([Id]);
GO

IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_LanguageStore_LanguageId'
    AND object_id = OBJECT_ID('[dbo].[LanguageStore]'))
BEGIN
	CREATE INDEX [IX_LanguageStore_LanguageId] ON [dbo].[LanguageStore](LanguageId)
END
GO
--Table: Country > Address, StoreBankAccount, City, Account
ALTER TABLE [dbo].[Country]
ADD IdTemp UNIQUEIDENTIFIER NOT NULL DEFAULT(NEWID());
GO

ALTER TABLE [dbo].[Account]
DROP CONSTRAINT FK_Account_Country_CountryId;
GO
ALTER TABLE [dbo].[Account]
ADD CountryIdTemp UNIQUEIDENTIFIER
GO
ALTER TABLE [dbo].[Address]
DROP CONSTRAINT FK_Address_Country_CountryId;
GO
ALTER TABLE [dbo].[Address]
ADD CountryIdTemp UNIQUEIDENTIFIER
GO
ALTER TABLE [dbo].[StoreBankAccount]
DROP CONSTRAINT FK_StoreBankAccount_Country_CountryId;
GO
ALTER TABLE [dbo].[StoreBankAccount]
ADD CountryIdTemp UNIQUEIDENTIFIER
GO
ALTER TABLE [dbo].[City]
ADD CountryIdTemp UNIQUEIDENTIFIER

UPDATE b
SET b.[CountryIdTemp] = a.[IdTemp]
FROM 
[dbo].[Account] b
INNER JOIN [dbo].[Country] a ON b.[CountryId] = a.[Id]
GO
UPDATE b
SET b.[CountryIdTemp] = a.[IdTemp]
FROM 
[dbo].[Address] b
INNER JOIN [dbo].[Country] a ON b.[CountryId] = a.[Id]
GO
UPDATE b
SET b.[CountryIdTemp] = a.[IdTemp]
FROM 
[dbo].[StoreBankAccount] b
INNER JOIN [dbo].[Country] a ON b.[CountryId] = a.[Id]
GO
UPDATE b
SET b.[CountryIdTemp] = a.[IdTemp]
FROM 
[dbo].[City] b
INNER JOIN [dbo].[Country] a ON b.[CountryId] = a.[Id]
GO

IF EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Account_CountryId'
    AND object_id = OBJECT_ID('[dbo].[Account]'))
BEGIN
    DROP INDEX [IX_Account_CountryId] ON [dbo].[Account]
END
GO
ALTER TABLE [dbo].[Account]
DROP COLUMN [CountryId]
GO
IF EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Address_CountryId'
    AND object_id = OBJECT_ID('[dbo].[Address]'))
BEGIN
    DROP INDEX [IX_Address_CountryId] ON [dbo].[Address]
END
GO
ALTER TABLE [dbo].[Address]
DROP COLUMN [CountryId]
GO
IF EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_StoreBankAccount_CountryId'
    AND object_id = OBJECT_ID('[dbo].[StoreBankAccount]'))
BEGIN
    DROP INDEX [IX_StoreBankAccount_CountryId] ON [dbo].[StoreBankAccount]
END
GO
ALTER TABLE [dbo].[StoreBankAccount]
DROP COLUMN [CountryId]
GO
ALTER TABLE [dbo].[City]
DROP COLUMN [CountryId]
GO

EXEC SP_RENAME '[dbo].[Account].[CountryIdTemp]', 'CountryId', 'COLUMN'
GO
EXEC SP_RENAME '[dbo].[Address].[CountryIdTemp]', 'CountryId', 'COLUMN'
GO
EXEC SP_RENAME '[dbo].[StoreBankAccount].[CountryIdTemp]', 'CountryId', 'COLUMN'
GO
EXEC SP_RENAME '[dbo].[City].[CountryIdTemp]', 'CountryId', 'COLUMN'
GO

ALTER TABLE [dbo].[Country]
DROP CONSTRAINT [PK_Country]
GO

ALTER TABLE [dbo].[Country]
DROP COLUMN [Id]
GO

EXEC SP_RENAME '[dbo].[Country].[IdTemp]', 'Id', 'COLUMN'
GO

ALTER TABLE [dbo].[Country]
ALTER COLUMN [Id] UNIQUEIDENTIFIER not null
GO

ALTER TABLE [dbo].[Country]
ADD CONSTRAINT PK_Country PRIMARY KEY ([Id]);
GO

ALTER TABLE [dbo].[Account]
ADD CONSTRAINT FK_Account_Country_CountryId
FOREIGN KEY ([CountryId]) REFERENCES [dbo].[Country]([Id]);
GO
ALTER TABLE [dbo].[Address]
ADD CONSTRAINT FK_Address_Country_CountryId
FOREIGN KEY ([CountryId]) REFERENCES [dbo].[Country]([Id]);
GO
ALTER TABLE [dbo].[StoreBankAccount]
ADD CONSTRAINT FK_StoreBankAccount_Country_CountryId
FOREIGN KEY ([CountryId]) REFERENCES [dbo].[Country]([Id]);
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Account_CountryId'
    AND object_id = OBJECT_ID('[dbo].[Account]'))
BEGIN
	CREATE INDEX [IX_Account_CountryId] ON [dbo].[Account](CountryId)
END
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Address_CountryId'
    AND object_id = OBJECT_ID('[dbo].[Address]'))
BEGIN
	CREATE INDEX [IX_Address_CountryId] ON [dbo].[Address](CountryId)
END
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_StoreBankAccount_CountryId'
    AND object_id = OBJECT_ID('[dbo].[StoreBankAccount]'))
BEGIN
	CREATE INDEX [IX_StoreBankAccount_CountryId] ON [dbo].[StoreBankAccount](CountryId)
END
GO

--Table: City, District, Ward, State > Address, StoreBankAccount | City, District, Ward, State
ALTER TABLE [dbo].[City]
ADD IdTemp UNIQUEIDENTIFIER NOT NULL DEFAULT(NEWID());
GO
ALTER TABLE [dbo].[District]
ADD IdTemp UNIQUEIDENTIFIER NOT NULL DEFAULT(NEWID());
GO
ALTER TABLE [dbo].[Ward]
ADD IdTemp UNIQUEIDENTIFIER NOT NULL DEFAULT(NEWID());
GO
ALTER TABLE [dbo].[State]
ADD IdTemp UNIQUEIDENTIFIER NOT NULL DEFAULT(NEWID());
GO

ALTER TABLE [dbo].[Address]
DROP CONSTRAINT FK_Address_City_CityId;
GO
ALTER TABLE [dbo].[StoreBankAccount]
DROP CONSTRAINT FK_StoreBankAccount_City_CityId;
GO
ALTER TABLE [dbo].[Address]
DROP CONSTRAINT FK_Address_District_DistrictId;
GO
ALTER TABLE [dbo].[Address]
DROP CONSTRAINT FK_Address_Ward_WardId;
GO
ALTER TABLE [dbo].[Address]
DROP CONSTRAINT FK_Address_State_StateId;
GO

ALTER TABLE [dbo].[Address]
ADD CityIdTemp UNIQUEIDENTIFIER
GO
ALTER TABLE [dbo].[StoreBankAccount]
ADD CityIdTemp UNIQUEIDENTIFIER
GO
ALTER TABLE [dbo].[Address]
ADD DistrictIdTemp UNIQUEIDENTIFIER
GO
ALTER TABLE [dbo].[Address]
ADD WardIdTemp UNIQUEIDENTIFIER
GO
ALTER TABLE [dbo].[Address]
ADD StateIdTemp UNIQUEIDENTIFIER
GO
ALTER TABLE [dbo].[District]
ADD CityIdTemp UNIQUEIDENTIFIER
GO
ALTER TABLE [dbo].[Ward]
ADD CityIdTemp UNIQUEIDENTIFIER
GO
ALTER TABLE [dbo].[Ward]
ADD DistrictIdTemp UNIQUEIDENTIFIER
GO

UPDATE b
SET b.[CityIdTemp] = a.[IdTemp]
FROM 
[dbo].[Address] b
INNER JOIN [dbo].[City] a ON b.[CityId] = a.[Id]
GO
UPDATE b
SET b.[CityIdTemp] = a.[IdTemp]
FROM 
[dbo].[StoreBankAccount] b
INNER JOIN [dbo].[City] a ON b.[CityId] = a.[Id]
GO
UPDATE b
SET b.[DistrictIdTemp] = a.[IdTemp]
FROM 
[dbo].[Address] b
INNER JOIN [dbo].[District] a ON b.[DistrictId] = a.[Id]
GO
UPDATE b
SET b.[WardIdTemp] = a.[IdTemp]
FROM 
[dbo].[Address] b
INNER JOIN [dbo].[Ward] a ON b.[WardId] = a.[Id]
GO
UPDATE b
SET b.[StateIdTemp] = a.[IdTemp]
FROM 
[dbo].[Address] b
INNER JOIN [dbo].[State] a ON b.[StateId] = a.[Id]
GO
UPDATE b
SET b.[CityIdTemp] = a.[IdTemp]
FROM 
[dbo].[District] b
INNER JOIN [dbo].[City] a ON b.[CityId] = a.[Id]
GO
UPDATE b
SET b.[CityIdTemp] = a.[IdTemp]
FROM 
[dbo].[Ward] b
INNER JOIN [dbo].[City] a ON b.[CityId] = a.[Id]
GO
UPDATE b
SET b.[DistrictIdTemp] = a.[IdTemp]
FROM 
[dbo].[Ward] b
INNER JOIN [dbo].[District] a ON b.[DistrictId] = a.[Id]
GO

IF EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Address_CityId'
    AND object_id = OBJECT_ID('[dbo].[Address]'))
BEGIN
    DROP INDEX [IX_Address_CityId] ON [dbo].[Address]
END
GO
ALTER TABLE [dbo].[Address]
DROP COLUMN [CityId]
GO
IF EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_StoreBankAccount_CityId'
    AND object_id = OBJECT_ID('[dbo].[StoreBankAccount]'))
BEGIN
    DROP INDEX [IX_StoreBankAccount_CityId] ON [dbo].[StoreBankAccount]
END
GO
ALTER TABLE [dbo].[StoreBankAccount]
DROP COLUMN [CityId]
GO
IF EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Address_DistrictId'
    AND object_id = OBJECT_ID('[dbo].[Address]'))
BEGIN
    DROP INDEX [IX_Address_DistrictId] ON [dbo].[Address]
END
GO
ALTER TABLE [dbo].[Address]
DROP COLUMN [DistrictId]
GO
IF EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Address_WardId'
    AND object_id = OBJECT_ID('[dbo].[Address]'))
BEGIN
    DROP INDEX [IX_Address_WardId] ON [dbo].[Address]
END
GO
ALTER TABLE [dbo].[Address]
DROP COLUMN [WardId]
GO
IF EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Address_StateId'
    AND object_id = OBJECT_ID('[dbo].[Address]'))
BEGIN
    DROP INDEX [IX_Address_StateId] ON [dbo].[Address]
END
GO
ALTER TABLE [dbo].[Address]
DROP COLUMN [StateId]
GO
ALTER TABLE [dbo].[District]
DROP COLUMN [CityId]
GO
ALTER TABLE [dbo].[Ward]
DROP COLUMN [CityId]
GO
ALTER TABLE [dbo].[Ward]
DROP COLUMN [DistrictId]
GO

EXEC SP_RENAME '[dbo].[Address].[CityIdTemp]', 'CityId', 'COLUMN'
GO
EXEC SP_RENAME '[dbo].[StoreBankAccount].[CityIdTemp]', 'CityId', 'COLUMN'
GO
EXEC SP_RENAME '[dbo].[Address].[DistrictIdTemp]', 'DistrictId', 'COLUMN'
GO
EXEC SP_RENAME '[dbo].[Address].[WardIdTemp]', 'WardId', 'COLUMN'
GO
EXEC SP_RENAME '[dbo].[Address].[StateIdTemp]', 'StateId', 'COLUMN'
GO
EXEC SP_RENAME '[dbo].[District].[CityIdTemp]', 'CityId', 'COLUMN'
GO
EXEC SP_RENAME '[dbo].[Ward].[CityIdTemp]', 'CityId', 'COLUMN'
GO
EXEC SP_RENAME '[dbo].[Ward].[DistrictIdTemp]', 'DistrictId', 'COLUMN'
GO

ALTER TABLE [dbo].[City]
DROP CONSTRAINT [PK_City]
GO
ALTER TABLE [dbo].[District]
DROP CONSTRAINT [PK_District]
GO
ALTER TABLE [dbo].[Ward]
DROP CONSTRAINT [PK_Ward]
GO
ALTER TABLE [dbo].[State]
DROP CONSTRAINT [PK_State]
GO

ALTER TABLE [dbo].[City]
DROP COLUMN [Id]
GO
ALTER TABLE [dbo].[District]
DROP COLUMN [Id]
GO
ALTER TABLE [dbo].[Ward]
DROP COLUMN [Id]
GO
ALTER TABLE [dbo].[State]
DROP COLUMN [Id]
GO

EXEC SP_RENAME '[dbo].[City].[IdTemp]', 'Id', 'COLUMN'
GO
EXEC SP_RENAME '[dbo].[District].[IdTemp]', 'Id', 'COLUMN'
GO
EXEC SP_RENAME '[dbo].[Ward].[IdTemp]', 'Id', 'COLUMN'
GO
EXEC SP_RENAME '[dbo].[State].[IdTemp]', 'Id', 'COLUMN'
GO

ALTER TABLE [dbo].[City]
ALTER COLUMN [Id] UNIQUEIDENTIFIER not null
GO
ALTER TABLE [dbo].[District]
ALTER COLUMN [Id] UNIQUEIDENTIFIER not null
GO
ALTER TABLE [dbo].[Ward]
ALTER COLUMN [Id] UNIQUEIDENTIFIER not null
GO
ALTER TABLE [dbo].[State]
ALTER COLUMN [Id] UNIQUEIDENTIFIER not null
GO

ALTER TABLE [dbo].[City]
ADD CONSTRAINT PK_City PRIMARY KEY ([Id]);
GO
ALTER TABLE [dbo].[District]
ADD CONSTRAINT PK_District PRIMARY KEY ([Id]);
GO
ALTER TABLE [dbo].[Ward]
ADD CONSTRAINT PK_Ward PRIMARY KEY ([Id]);
GO
ALTER TABLE [dbo].[State]
ADD CONSTRAINT PK_State PRIMARY KEY ([Id]);
GO

ALTER TABLE [dbo].[Address]
ADD CONSTRAINT FK_Address_City_CityId
FOREIGN KEY ([CityId]) REFERENCES [dbo].[City]([Id]);
GO
ALTER TABLE [dbo].[StoreBankAccount]
ADD CONSTRAINT FK_StoreBankAccount_City_CityId
FOREIGN KEY ([CityId]) REFERENCES [dbo].[City]([Id]);
GO
ALTER TABLE [dbo].[Address]
ADD CONSTRAINT FK_Address_District_DistrictId
FOREIGN KEY ([DistrictId]) REFERENCES [dbo].[District]([Id]);
GO
ALTER TABLE [dbo].[Address]
ADD CONSTRAINT FK_Address_Ward_WardId
FOREIGN KEY ([WardId]) REFERENCES [dbo].[Ward]([Id]);
GO
ALTER TABLE [dbo].[Address]
ADD CONSTRAINT FK_Address_State_StateId
FOREIGN KEY ([StateId]) REFERENCES [dbo].[State]([Id]);
GO

IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Address_CityId'
    AND object_id = OBJECT_ID('[dbo].[Address]'))
BEGIN
	CREATE INDEX [IX_Address_CityId] ON [dbo].[Address](CityId)
END
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Address_DistrictId'
    AND object_id = OBJECT_ID('[dbo].[Address]'))
BEGIN
	CREATE INDEX [IX_Address_DistrictId] ON [dbo].[Address](DistrictId)
END
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Address_WardId'
    AND object_id = OBJECT_ID('[dbo].[Address]'))
BEGIN
	CREATE INDEX [IX_Address_WardId] ON [dbo].[Address](WardId)
END
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Address_StateId'
    AND object_id = OBJECT_ID('[dbo].[Address]'))
BEGIN
	CREATE INDEX [IX_Address_StateId] ON [dbo].[Address](StateId)
END
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_StoreBankAccount_CityId'
    AND object_id = OBJECT_ID('[dbo].[StoreBankAccount]'))
BEGIN
	CREATE INDEX [IX_StoreBankAccount_CityId] ON [dbo].[StoreBankAccount](CityId)
END
GO

--Table: Currency > Store
ALTER TABLE [dbo].[Currency]
ADD IdTemp UNIQUEIDENTIFIER NOT NULL DEFAULT(NEWID());
GO

ALTER TABLE [dbo].[Store]
DROP CONSTRAINT FK_Store_Currency_CurrencyId;
GO
ALTER TABLE [dbo].[Store]
ADD CurrencyIdTemp UNIQUEIDENTIFIER
GO

UPDATE b
SET b.[CurrencyIdTemp] = a.[IdTemp]
FROM 
[dbo].[Store] b
INNER JOIN [dbo].[Currency] a ON b.[CurrencyId] = a.[Id]
GO

IF EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Store_CurrencyId'
    AND object_id = OBJECT_ID('[dbo].[Store]'))
BEGIN
    DROP INDEX [IX_Store_CurrencyId] ON [dbo].[Store]
END
GO
ALTER TABLE [dbo].[Store]
DROP COLUMN [CurrencyId]
GO

EXEC SP_RENAME '[dbo].[Store].[CurrencyIdTemp]', 'CurrencyId', 'COLUMN'
GO

ALTER TABLE [dbo].[Currency]
DROP CONSTRAINT [PK_Currency]
GO

ALTER TABLE [dbo].[Currency]
DROP COLUMN [Id]
GO

EXEC SP_RENAME '[dbo].[Currency].[IdTemp]', 'Id', 'COLUMN'
GO

ALTER TABLE [dbo].[Currency]
ALTER COLUMN [Id] UNIQUEIDENTIFIER not null
GO

ALTER TABLE [dbo].[Currency]
ADD CONSTRAINT PK_Currency PRIMARY KEY ([Id]);
GO

ALTER TABLE [dbo].[Store]
ADD CONSTRAINT FK_Store_Currency_CurrencyId
FOREIGN KEY ([CurrencyId]) REFERENCES [dbo].[Currency]([Id]);
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_Store_CurrencyId'
    AND object_id = OBJECT_ID('[dbo].[Store]'))
BEGIN
	CREATE INDEX [IX_Store_CurrencyId] ON [dbo].[Store](CurrencyId)
END
GO

--Table: AccountTransfer > 
ALTER TABLE [dbo].[AccountTransfer]
ADD IdTemp UNIQUEIDENTIFIER NOT NULL DEFAULT(NEWID());
GO

ALTER TABLE [dbo].[AccountTransfer]
DROP CONSTRAINT [PK_AccountTransfer]
GO

ALTER TABLE [dbo].[AccountTransfer]
DROP COLUMN [Id]
GO

EXEC SP_RENAME '[dbo].[AccountTransfer].[IdTemp]', 'Id', 'COLUMN'
GO

ALTER TABLE [dbo].[AccountTransfer]
ALTER COLUMN [Id] UNIQUEIDENTIFIER not null
GO

ALTER TABLE [dbo].[AccountTransfer]
ADD CONSTRAINT PK_AccountTransfer PRIMARY KEY ([Id]);
GO

--Table: UserActivity > 
ALTER TABLE [dbo].[UserActivity]
ADD IdTemp UNIQUEIDENTIFIER NOT NULL DEFAULT(NEWID());
GO

ALTER TABLE [dbo].[UserActivity]
DROP CONSTRAINT [PK_UserActivity]
GO

ALTER TABLE [dbo].[UserActivity]
DROP COLUMN [Id]
GO

EXEC SP_RENAME '[dbo].[UserActivity].[IdTemp]', 'Id', 'COLUMN'
GO

ALTER TABLE [dbo].[UserActivity]
ALTER COLUMN [Id] UNIQUEIDENTIFIER not null
GO

ALTER TABLE [dbo].[UserActivity]
ADD CONSTRAINT PK_UserActivity PRIMARY KEY ([Id]);
GO

--Table: Channel > ProductChannel
ALTER TABLE [dbo].[Channel]
ADD IdTemp UNIQUEIDENTIFIER NOT NULL DEFAULT(NEWID());
GO

UPDATE c
SET c.IdTemp = CONVERT(uniqueidentifier,CONVERT(VARBINARY(12), 'TableChannel') + CONVERT(binary(4), c.Id))
FROM [dbo].[Channel] c
GO

ALTER TABLE [dbo].[ProductChannel]
DROP CONSTRAINT FK_ProductChannel_Channel_ChannelId;
GO
ALTER TABLE [dbo].[ProductChannel]
ADD ChannelIdTemp UNIQUEIDENTIFIER
GO

UPDATE b
SET b.[ChannelIdTemp] = a.[IdTemp]
FROM 
[dbo].[ProductChannel] b
INNER JOIN [dbo].[Channel] a ON b.[ChannelId] = a.[Id]
GO

IF EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_ProductChannel_ChannelId'
    AND object_id = OBJECT_ID('[dbo].[ProductChannel]'))
BEGIN
    DROP INDEX [IX_ProductChannel_ChannelId] ON [dbo].[ProductChannel]
END
GO
ALTER TABLE [dbo].[ProductChannel]
DROP COLUMN [ChannelId]
GO

EXEC SP_RENAME '[dbo].[ProductChannel].[ChannelIdTemp]', 'ChannelId', 'COLUMN'
GO

ALTER TABLE [dbo].[Channel]
DROP CONSTRAINT [PK_Channel]
GO

ALTER TABLE [dbo].[Channel]
DROP COLUMN [Id]
GO

EXEC SP_RENAME '[dbo].[Channel].[IdTemp]', 'Id', 'COLUMN'
GO

ALTER TABLE [dbo].[Channel]
ALTER COLUMN [Id] UNIQUEIDENTIFIER not null
GO

ALTER TABLE [dbo].[Channel]
ADD CONSTRAINT PK_Channel PRIMARY KEY ([Id]);
GO

ALTER TABLE [dbo].[ProductChannel]
ADD CONSTRAINT FK_ProductChannel_Channel_ChannelId
FOREIGN KEY ([ChannelId]) REFERENCES [dbo].[Channel]([Id]);
GO
IF NOT EXISTS (SELECT *  FROM sys.indexes  WHERE name='IX_ProductChannel_ChannelId'
    AND object_id = OBJECT_ID('[dbo].[ProductChannel]'))
BEGIN
	CREATE INDEX [IX_ProductChannel_ChannelId] ON [dbo].[ProductChannel](ChannelId)
END
GO