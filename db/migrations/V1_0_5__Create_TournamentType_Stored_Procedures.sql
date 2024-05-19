CREATE PROCEDURE [dbo].[usp_TournamentType_GetAll]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
		tt.TournamentTypeID,
		tt.Name
	FROM
		[dbo].[TournamentType] tt;
END
GO

CREATE PROCEDURE [dbo].[usp_TournamentType_GetByTournamentTypeId]
	@tournamentTypeId INT
AS
BEGIN
	SET NOCOUNT ON;

	IF (@tournamentTypeId <= 0 OR @tournamentTypeId IS NULL)
		BEGIN
			RAISERROR ('The parameter @tournamentTypeId for procedure [dbo].[usp_TournamentType_GetByTournamentTypeId] may not be NULL.', 16, 1);
		END

	SELECT
		tt.TournamentTypeID,
		tt.Name
	FROM
		[dbo].[TournamentType] tt
	WHERE
		tt.TournamentTypeID = @tournamentTypeId;
END
GO

CREATE PROCEDURE [dbo].[usp_TournamentType_Insert]
	@name NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	IF (@name IS NULL OR LEN(@name) = 0)
		BEGIN
			RAISERROR ('The parameter @name for procedure [dbo].[usp_TournamentType_Insert] may not be NULL.', 16, 1);
		END

	INSERT INTO [dbo].[TournamentType] (Name)
	VALUES (@name);

	SELECT SCOPE_IDENTITY() AS tournamentTypeId;
END
GO

CREATE PROCEDURE [dbo].[usp_TournamentType_Update]
	@tournamentTypeId INT,
	@name NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	IF (@tournamentTypeId <= 0 OR @tournamentTypeId IS NULL)
		BEGIN
			RAISERROR ('The parameter @tournamentTypeId for procedure [dbo].[usp_TournamentType_Update] may not be NULL.', 16, 1);
		END

	IF (@name IS NULL OR LEN(@name) = 0)
		BEGIN
			RAISERROR ('The parameter @name for procedure [dbo].[usp_TournamentType_Update] may not be NULL.', 16, 1);
		END

	UPDATE [dbo].[TournamentType]
	SET
		Name = @name
	WHERE
		TournamentTypeID = @tournamentTypeId;
END
GO

CREATE PROCEDURE [dbo].[usp_TournamentType_Delete]
	@tournamentTypeId INT
AS
BEGIN
	SET NOCOUNT ON;

	IF (@tournamentTypeId <= 0 OR @tournamentTypeId IS NULL)
		BEGIN
			RAISERROR ('The parameter @tournamentTypeId for procedure [dbo].[usp_TournamentType_Delete] may not be NULL.', 16, 1);
		END

	DELETE
	FROM
		[dbo].[TournamentType]
	WHERE
		TournamentTypeID = @tournamentTypeId;
END
GO
