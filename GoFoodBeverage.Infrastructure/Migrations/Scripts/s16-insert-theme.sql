
IF NOT EXISTS (SELECT Id FROM [dbo].[Theme]) 
BEGIN	
	INSERT INTO [dbo].[Theme] (Id, Name, Description,Tags,Thumbnail)
	VALUES
		('921016fe-d34e-4192-beb8-15d775d0ee5b', N'The Default', 'Description of Default Theme','Age verifier,Infinite scroll,Sticky header','assets/images/default.png'),
		('46565f44-c3e2-449d-8d58-3850a95ffba7', N'Ciao', 'Description of Ciao Theme','Quick view,Mega menu, Sticky header','assets/images/ciao.png'),
        ('526cb94e-3973-4fba-b4f4-80b53a7db652', N'Frappe', 'Lorem Ipsum is simply dummy text of the printing and typesetting industry.','EU translations (EN FR IT DE ES),Store locator,Quick view,Book Table,Qick buy','assets/images/frappe.png')
END

