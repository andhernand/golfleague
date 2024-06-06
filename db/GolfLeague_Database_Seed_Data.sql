INSERT INTO [dbo].[Golfer]
	(FirstName, LastName, Email, JoinDate, Handicap)
VALUES (N'Melissa', N'McKenzie', N'Morris45@gmail.com', CAST('2005-05-28' AS DATE), NULL),
	   (N'Michael', N'Stone', N'Tanya49@gmail.com', CAST('2009-05-31' AS DATE), 3),
	   (N'Brenda', N'Nolan', N'Lee_Ziemann10@hotmail.com', CAST('2006-08-12' AS DATE), 0),
	   (N'Helen', N'Branch', N'Kristi.Green@gmail.com', CAST('2011-12-06' AS DATE), NULL),
	   (N'Sarah', N'Beck ', N'Kathleen34@yahoo.com', CAST('2010-01-24' AS DATE), NULL),
	   (N'Sandra', N'Burton', N'Terry_Gibson@yahoo.com', CAST('2013-07-09' AS DATE), 12),
	   (N'William', N'Cooper', N'Cameron92@hotmail.com', CAST('2008-03-05' AS DATE), NULL),
	   (N'Thomas', N'Spence', N'Esther.Cummings@gmail.com', CAST('2006-06-22' AS DATE), NULL),
	   (N'Barbara', N'Olson', N'Kurt_Bergstrom@yahoo.com', CAST('2013-07-29' AS DATE), 9),
	   (N'Robert', N'Pollard', N'Randolph.Quigley35@gmail.com', CAST('2013-08-13' AS DATE), NULL),
	   (N'Thomas', N'Sexton', N'Kristina_Fahey8@gmail.com', CAST('2008-07-28' AS DATE), NULL),
	   (N'Daniel', N'Wilcox', N'Grant57@yahoo.com', CAST('2009-05-18' AS DATE), NULL),
	   (N'Thomas', N'Schmidt', N'Glenn.King@hotmail.com', CAST('2009-04-07' AS DATE), NULL),
	   (N'Deborah', N'Bridges', N'Lorene29@hotmail.com', CAST('2007-03-23' AS DATE), 28),
	   (N'Betty', N'Young', N'Marcia_Baumbach@hotmail.com', CAST('2009-04-17' AS DATE), NULL),
	   (N'Jane ', N'Gilmore', N'Devin_Sporer47@hotmail.com', CAST('2007-05-30' AS DATE), 4),
	   (N'William', N'Taylor', N'Michele_Hamill3@yahoo.com', CAST('2007-11-27' AS DATE), NULL),
	   (N'Robert', N'Reed ', N'Agnes_Schmidt31@yahoo.com', CAST('2005-08-05' AS DATE), 9),
	   (N'Carolyn', N'Willis', N'Bradley.Bradtke@yahoo.com', CAST('2011-01-14' AS DATE), NULL),
	   (N'Susan', N'Kent ', N'Boyd.Bednar@yahoo.com', CAST('2010-10-07' AS DATE), NULL);
GO

INSERT INTO [dbo].[Tournament]
	([Name], [Format])
VALUES (N'Leeston', N'Best Ball'),
	   (N'Kaiapoi', N'Stoke Play'),
	   (N'WestCoast', N'Match Play'),
	   (N'Canterbury', N'Alternate Shot'),
	   (N'Otago', N'Scramble');
GO

INSERT INTO [dbo].[TournamentParticipation]
	([GolferId], [TournamentId], [Year], [Score])
VALUES (1, 1, 2014, 73),
	   (6, 1, 2015, 75),
	   (6, 2, 2015, 71),
	   (6, 3, 2015, 72),
	   (7, 4, 2013, 79),
	   (7, 4, 2015, 80),
	   (7, 5, 2014, 77),
	   (7, 5, 2015, 71),
	   (8, 2, 2015, 72),
	   (8, 5, 2013, 75),
	   (9, 1, 2014, 74),
	   (9, 4, 2014, 72),
	   (10, 1, 2013, 72),
	   (10, 1, 2014, 70),
	   (10, 1, 2015, 69),
	   (17, 1, 2015, 68),
	   (17, 2, 2013, 76),
	   (17, 3, 2014, 72),
	   (17, 3, 2015, 72),
	   (17, 4, 2013, 72),
	   (17, 4, 2015, 71),
	   (17, 5, 2013, 78),
	   (17, 5, 2014, 68),
	   (17, 5, 2015, 70);
GO
