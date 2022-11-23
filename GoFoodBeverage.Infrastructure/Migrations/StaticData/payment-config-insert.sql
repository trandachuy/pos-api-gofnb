
INSERT INTO [dbo].[Address]
           ([Id]
           ,[CountryId]
           ,[CityId]
           ,[DistrictId]
           ,[WardId]
           ,[Address1]
           ,[Address2]
           ,[CityTown]
           ,[PostalCode]
           ,[LastSavedUser]
           ,[LastSavedTime]
           ,[CreatedUser]
           ,[CreatedTime]
           ,[StateId])
     VALUES
           ('0BE12438-BA65-46F8-8376-838372B6C68B'
           ,243
           ,1
           ,22
           ,296
           ,'60A Trường Sơn'
           ,NULL
           ,NULL
           ,NULL
           ,NULL
           ,'2022-08-26 03:14:57.6367321'
           ,NULL
           ,'2022-08-26 03:14:57.6367321'
           ,NULL)

INSERT INTO [dbo].[Account]
           ([Id]
           ,[AccountTypeId]
           ,[Username]
           ,[Password]
           ,[ValidateCode]
           ,[EmailConfirmed]
           ,[LastSavedUser]
           ,[LastSavedTime]
           ,[CreatedUser]
           ,[CreatedTime])
     VALUES
           ('EF560FEB-16D3-4906-AB0A-11A48E172F55'
           ,'C3408968-2942-4085-959D-A0EC09BB3952'
           ,'truong.tran@mediastep.com'
           ,'AQAAAAEAACcQAAAAEPqry/6xAjW4dW5D7wfaR4TQ18DiGmGLHqmQLUAfNNcKaiAhNdB0fDwSpTQBPx/27A==' --1234567
           ,'aaa3j'
           ,1
           ,NULL
           ,'2022-08-26 03:14:57.6367321'
           ,NULL
           ,'2022-08-26 03:14:57.6367321')


INSERT INTO [dbo].[Store]
           ([Id]
           ,[InitialStoreAccountId]
           ,[CurrencyId]
           ,[AddressId]
           ,[BusinessAreaId]
           ,[Title]
           ,[LastSavedUser]
           ,[LastSavedTime]
           ,[CreatedUser]
           ,[CreatedTime]
           ,[IsActivated]
           ,[Logo]
           ,[IsAutoPrintStamp]
           ,[IsStoreHasKitchen])
     VALUES
           ('0605957D-DA43-4310-8128-550D9B66F9CB'
           ,'EF560FEB-16D3-4906-AB0A-11A48E172F55'
           ,110
           ,'0BE12438-BA65-46F8-8376-838372B6C68B'
           ,'33408968-2942-4085-959D-A0EC09BB3952'
           ,'FNB store'
           ,NULL
            ,'2022-08-26 03:14:57.6367321'
            ,NULL
            ,'2022-08-26 03:14:57.6367321'
           ,0
           ,NULL
           ,0
           ,0)

INSERT INTO [dbo].[PaymentConfig]
           ([Id]
           ,[StoreId]
           ,[PaymentMethodId]
           ,[PaymentMethodEnumId]
           ,[PartnerCode]
           ,[AccessKey]
           ,[SecretKey]
           ,[QRCode]
           ,[IsActivated]
           ,[LastSavedUser]
           ,[LastSavedTime]
           ,[CreatedUser]
           ,[CreatedTime]
           ,[IsAuthenticated])
     VALUES
           ('58F58E35-8E30-4456-99AE-269744C3ABCE'
           ,'0605957D-DA43-4310-8128-550D9B66F9CB'
           ,'BC3EC865-05A4-4E62-9B03-884A1360D5CA'
           ,4
           ,'XAPFAQUZ'
           ,NULL
           ,'SNNGBOYXJTBIUCYUSVWJAPMJIRCULHMT'
           ,NULL
           ,1
           ,NULL
            ,'2022-08-26 03:14:57.6367321'
		   ,NULL
            ,'2022-08-26 03:14:57.6367321'
           ,1)
