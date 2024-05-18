IF DB_ID(N'GolfLeague') IS NULL
BEGIN
	CREATE DATABASE [GolfLeague];
END;
GO

IF NOT EXISTS (SELECT 1 FROM [master].[sys].[server_principals] WHERE [name] = N'golfleagueapi' and [type] IN ('C','E', 'G', 'K', 'S', 'U'))
BEGIN
	CREATE LOGIN [golfleagueapi] WITH PASSWORD=N'$(varApiPassword)', DEFAULT_DATABASE=[GolfLeague];
END;
GO
