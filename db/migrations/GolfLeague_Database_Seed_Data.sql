INSERT INTO [dbo].[MemberType]
	([Name], [Fee])
VALUES
	(N'Junior', 150),
	(N'Senior', 300),
	(N'Social', 50);
GO

INSERT INTO [dbo].[Member]
	([LastName], [FirstName], [MemberTypeID], [Phone], [Handicap], [JoinDate], [Coach], [Gender])
VALUES
	(N'McKenzie', N'Melissa', 1, '5555963270', 30, CAST('2005-05-28' AS DATE), NULL, N'F'),
	(N'Stone', N'Michael', 2, '5555983223', 30, CAST('2009-05-31' AS DATE), NULL, N'M'),
	(N'Nolan', N'Brenda', 2, '5555442649', 11, CAST('2006-08-12' AS DATE), NULL, N'F'),
	(N'Branch', N'Helen', 3, '5555589419', NULL, CAST('2011-12-06' AS DATE), NULL, N'F'),
	(N'Beck ', N'Sarah', 3, '5555226596', NULL, CAST('2010-01-24' AS DATE), NULL, N'F'),
	(N'Burton', N'Sandra', 1, '5555244493', 26, CAST('2013-07-09' AS DATE), NULL, N'F'),
	(N'Cooper', N'William', 2, '5555722954', 14, CAST('2008-03-05' AS DATE), NULL, N'M'),
	(N'Spence', N'Thomas', 2, '5555697720', 10, CAST('2006-06-22' AS DATE), NULL, N'M'),
	(N'Olson', N'Barbara', 2, '5555370186', 16, CAST('2013-07-29' AS DATE), NULL, N'F'),
	(N'Pollard', N'Robert', 1, '5555617681', 19, CAST('2013-08-13' AS DATE), NULL, N'M'),
	(N'Sexton', N'Thomas', 2, '5555268936', 26, CAST('2008-07-28' AS DATE), NULL, N'M'),
	(N'Wilcox', N'Daniel', 2, '5555665393', 3, CAST('2009-05-18' AS DATE), NULL, N'M'),
	(N'Schmidt', N'Thomas', 2, '5555867492', 25, CAST('2009-04-07' AS DATE), NULL, N'M'),
	(N'Bridges', N'Deborah', 2, '5555279087', 12, CAST('2007-03-23' AS DATE), NULL, N'F'),
	(N'Young', N'Betty', 2, '5555507813', 21, CAST('2009-04-17' AS DATE), NULL, N'F'),
	(N'Gilmore', N'Jane ', 1, '5555459558', 5, CAST('2007-05-30' AS DATE), NULL, N'F'),
	(N'Taylor', N'William', 2, '5555137353', 7, CAST('2007-11-27' AS DATE), NULL, N'M'),
	(N'Reed ', N'Robert', 2, '5555994664', 3, CAST('2005-08-05' AS DATE), NULL, N'M'),
	(N'Willis', N'Carolyn', 1, '5555688378', 29, CAST('2011-01-14' AS DATE), NULL, N'F'),
	(N'Kent ', N'Susan', 3, '5555707217', NULL, CAST('2010-10-07' AS DATE), NULL, N'F');
GO

INSERT INTO [dbo].[TournamentType]
	([Name])
VALUES
	(N'Social'),
	(N'Open');
GO

INSERT INTO [dbo].[Tournament]
	([Name], [TournamentTypeID])
VALUES
	(N'Leeston', 1),
	(N'Kaiapoi', 1),
	(N'WestCoast', 2),
	(N'Canterbury', 2),
	(N'Otago', 2);
GO

INSERT INTO [dbo].[TournamentParticipation]
	([MemberID], [TournamentID], [Year])
VALUES
	(1, 1, 2014),
	(6, 1, 2015),
	(6, 2, 2015),
	(6, 3, 2015),
	(7, 4, 2013),
	(7, 4, 2015),
	(7, 5, 2014),
	(7, 5, 2015),
	(8, 2, 2015),
	(8, 5, 2013),
	(9, 1, 2014),
	(9, 4, 2014),
	(10, 1, 2013),
	(10, 1, 2014),
	(10, 1, 2015),
	(17, 1, 2015),
	(17, 2, 2013),
	(17, 3, 2014),
	(17, 3, 2015),
	(17, 4, 2013),
	(17, 4, 2015),
	(17, 5, 2013),
	(17, 5, 2014),
	(17, 5, 2015);
GO

UPDATE [dbo].[Member] SET [Coach] = 3 WHERE [MemberID] = 1;
GO
UPDATE [dbo].[Member] SET [Coach] = 3 WHERE [MemberID] = 6;
GO
UPDATE [dbo].[Member] SET [Coach] = 3 WHERE [MemberID] = 7;
GO
UPDATE [dbo].[Member] SET [Coach] = 7 WHERE [MemberID] = 10;
GO
UPDATE [dbo].[Member] SET [Coach] = 7 WHERE [MemberID] = 11;
GO
UPDATE [dbo].[Member] SET [Coach] = 3 WHERE [MemberID] = 13;
GO
UPDATE [dbo].[Member] SET [Coach] = 7 WHERE [MemberID] = 14;
GO
UPDATE [dbo].[Member] SET [Coach] = 3 WHERE [MemberID] = 16;
GO
UPDATE [dbo].[Member] SET [Coach] = 7 WHERE [MemberID] = 17;
GO
UPDATE [dbo].[Member] SET [Coach] = 7 WHERE [MemberID] = 18;
GO
