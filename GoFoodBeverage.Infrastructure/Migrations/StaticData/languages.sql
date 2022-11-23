DELETE [dbo].[Language]

SET IDENTITY_INSERT [dbo].[Language] ON;
GO
INSERT INTO [dbo].[Language] (Id, Name, Emoji, IsDefault)
VALUES
(1, 'Vietnamese', 'VN', 1),
(2, 'English', 'GB', 1),
(3, 'Chinese', 'CN', 0),
(4, 'Thailand', 'TH', 0),
(5, 'Malaysian', 'MY', 0),
(6, 'Indonesian', 'ID', 0),
(7, 'Japanese', 'JP', 0),
(8, 'Korean', 'KR', 0);

SET IDENTITY_INSERT [dbo].[Language] OFF;
GO
