CREATE PROCEDURE [dbo].[usp_Tournament_GetAll]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
		t.TournamentID,
		t.Name,
		t.TournamentTypeID
	FROM
		[dbo].[Tournament] t;
END
GO

CREATE PROCEDURE [dbo].[usp_Tournament_GetByTournamentId]
	@tournamentId INT
AS
BEGIN
	SET NOCOUNT ON;

	IF (@tournamentId <= 0 OR @tournamentId IS NULL)
		BEGIN
			RAISERROR ('The parameter @tournamentId for procedure [dbo].[usp_Tournament_GetByTournamentId] may not be NULL.', 16, 1);
		END

	SELECT
		t.TournamentID,
		t.Name,
		t.TournamentTypeID
	FROM
		[dbo].[Tournament] t
	WHERE
		t.TournamentID = @tournamentId;
END
GO

CREATE PROCEDURE [dbo].[usp_Tournament_GetByTournamentTypeId]
	@tournamentTypeId INT
AS
BEGIN
	SET NOCOUNT ON;

	IF (@tournamentTypeId <= 0 OR @tournamentTypeId IS NULL)
		BEGIN
			RAISERROR ('The parameter @tournamentTypeId for procedure [dbo].[usp_Tournament_GetByTournamentTypeId] may not be NULL.', 16, 1);
		END

	SELECT
		t.TournamentID,
		t.Name,
		t.TournamentTypeID
	FROM
		[dbo].[Tournament] t
	WHERE
		t.TournamentTypeID = @tournamentTypeId;
END
GO

CREATE PROCEDURE [dbo].[usp_Tournament_Insert]
	@name NVARCHAR(256),
	@tournamentTypeId INT
AS
BEGIN
	SET NOCOUNT ON;

	IF (@name IS NULL OR LEN(@name) = 0)
		BEGIN
			RAISERROR ('The parameter @name for procedure [dbo].[usp_Tournament_Insert] may not be NULL.', 16, 1);
		END

	IF (@tournamentTypeId <= 0 OR @tournamentTypeId IS NULL)
		BEGIN
			RAISERROR ('The parameter @tournamentTypeId for procedure [dbo].[usp_Tournament_Insert] may not be NULL.', 16, 1);
		END

	INSERT INTO [dbo].[Tournament] (Name, TournamentTypeID)
	OUTPUT INSERTED.TournamentTypeID
	VALUES (@name, @tournamentTypeId);
END
GO

CREATE PROCEDURE [dbo].[usp_Tournament_Update]
	@tournamentId INT,
	@name NVARCHAR(256),
	@tournamentTypeId INT
AS
BEGIN
	SET NOCOUNT ON;

	IF (@tournamentId <= 0 OR @tournamentId IS NULL)
		BEGIN
			RAISERROR ('The parameter @tournamentId for procedure [dbo].[usp_Tournament_Update] may not be NULL.', 16, 1);
		END

	IF (@name IS NULL OR LEN(@name) = 0)
		BEGIN
			RAISERROR ('The parameter @name for procedure [dbo].[usp_Tournament_Update] may not be NULL.', 16, 1);
		END

	IF (@tournamentTypeId <= 0 OR @tournamentTypeId IS NULL)
		BEGIN
			RAISERROR ('The parameter @tournamentTypeId for procedure [dbo].[usp_Tournament_Update] may not be NULL.', 16, 1);
		END

	UPDATE
		[dbo].[Tournament]
	SET
		Name = @name,
		TournamentTypeID = @tournamentTypeId
	WHERE
		TournamentID = @tournamentId;

END
GO

CREATE PROCEDURE [dbo].[usp_Tournament_Delete]
	@tournamentId INT
AS
BEGIN
	SET NOCOUNT ON;

	IF (@tournamentId <= 0 OR @tournamentId IS NULL)
		BEGIN
			RAISERROR ('The parameter @tournamentId for procedure [dbo].[usp_Tournament_Delete] may not be NULL.', 16, 1);
		END

	DELETE
	FROM
		[dbo].[Tournament]
	WHERE
		TournamentID = @tournamentId;
END
GO
