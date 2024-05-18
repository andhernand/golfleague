CREATE USER [golfleagueapi] FOR LOGIN [golfleagueapi] WITH DEFAULT_SCHEMA=[dbo];
GO

CREATE ROLE [golfleagueapipermissions];
GO

ALTER ROLE [golfleagueapipermissions] ADD MEMBER [golfleagueapi];
GO

GRANT SELECT, UPDATE, INSERT, DELETE, EXECUTE ON SCHEMA::DBO TO [golfleagueapipermissions];
GO
