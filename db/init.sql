IF (DB_ID(N'GolfLeague') IS NULL)
	BEGIN
		PRINT 'Creating the database.';
		CREATE DATABASE [GolfLeague];
	END
ELSE
	BEGIN
		PRINT 'Database already exists.';
	END;
GO

IF NOT EXISTS(SELECT 1 FROM [master].[sys].[server_principals] WHERE [name] = N'golfleagueapi' AND [type] IN ('C', 'E', 'G', 'K', 'S', 'U'))
	BEGIN
		PRINT 'Creating the login.';
		CREATE LOGIN [golfleagueapi] WITH PASSWORD =N'$(varApiPassword)', DEFAULT_DATABASE = [GolfLeague];
	END
ELSE
	BEGIN
		PRINT 'Login already exists.';
	END;
GO
