CREATE PROCEDURE [dbo].[usp_TournamentParticipation_Insert](
	@GolferId INT,
	@TournamentId INT,
	@Year NCHAR(4),
	@RowCount INT OUTPUT
)
AS
BEGIN
	SET NOCOUNT ON;

	-- Validate @GolferId
	IF @GolferId IS NULL OR @GolferId <= 0
		BEGIN
			THROW 50027, 'The GolferId parameter must have a positive value.', 1;
		END

	IF NOT EXISTS (SELECT 1 FROM [dbo].[Golfer] WHERE [GolferId] = @GolferId)
		BEGIN
			THROW 50028, 'The specified GolferId does not exist in the database.', 1;
		END

	-- Validate @TournamentId
	IF @TournamentId IS NULL OR @TournamentId <= 0
		BEGIN
			THROW 50029, 'The TournamentId parameter must have a positive value.', 1;
		END

	IF NOT EXISTS (SELECT 1 FROM [dbo].[Tournament] WHERE [TournamentId] = @TournamentId)
		BEGIN
			THROW 50030, 'The specified TournamentId does not exist in the database.', 1;
		END

	-- Validate @Year
	IF @Year IS NULL OR LEN(@Year) != 4
		BEGIN
			THROW 50031, 'The Year parameter must be a 4-character string.', 1;
		END

	BEGIN TRY
		BEGIN TRANSACTION;

		INSERT INTO [dbo].[TournamentParticipation] ([GolferId], [TournamentId], [Year])
		VALUES (@GolferId, @TournamentId, @Year);

		SET @RowCount = @@ROWCOUNT;

		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		SET @RowCount = 0;
		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;
		THROW;
	END CATCH;
END;
GO

CREATE PROCEDURE [dbo].[usp_TournamentParticipation_GetAll]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [GolferId], [TournamentId], [Year]
	FROM [dbo].[TournamentParticipation];
END;
GO

CREATE PROCEDURE [dbo].[usp_TournamentParticipation_GetByGolferIdTournamentIdYear](
	@GolferId INT,
	@TournamentId INT,
	@Year NVARCHAR(256)
)
AS
BEGIN
	SET NOCOUNT ON;

	-- Validate @GolferId
	IF @GolferId IS NULL OR @GolferId <= 0
		BEGIN
			THROW 50032, 'The GolferId parameter must have a positive value.', 1;
		END

	-- Validate @TournamentId
	IF @TournamentId IS NULL OR @TournamentId <= 0
		BEGIN
			THROW 50033, 'The TournamentId parameter must have a positive value.', 1;
		END

	-- Validate @Year
	IF @Year IS NULL OR LEN(@Year) != 4
		BEGIN
			THROW 50034, 'The Year parameter must be a 4-character string.', 1;
		END

	SELECT [GolferId], [TournamentId], [Year]
	FROM [dbo].[TournamentParticipation]
	WHERE GolferId = @GolferId
	  AND TournamentId = @TournamentId
	  AND Year = @Year;
END;
GO

CREATE PROCEDURE [dbo].[usp_TournamentParticipation_GetByGolferId](
	@GolferId INT
)
AS
BEGIN
	SET NOCOUNT ON;

	-- Validate @GolferId
	IF @GolferId IS NULL OR @GolferId <= 0
		BEGIN
			THROW 50035, 'The GolferId parameter must have a positive value.', 1;
		END

	SELECT [GolferId], [TournamentId], [Year]
	FROM [dbo].[TournamentParticipation]
	WHERE [GolferId] = @GolferId;
END;
GO

CREATE PROCEDURE [dbo].[usp_TournamentParticipation_GetByTournamentId](
	@TournamentId INT
)
AS
BEGIN
	SET NOCOUNT ON;

	-- Validate @TournamentId
	IF @TournamentId IS NULL OR @TournamentId <= 0
		BEGIN
			THROW 50036, 'The TournamentId parameter must have a positive value.', 1;
		END

	SELECT [GolferId], [TournamentId], [Year]
	FROM [dbo].[TournamentParticipation]
	WHERE [TournamentId] = @TournamentId;
END;
GO

CREATE PROCEDURE [dbo].[usp_TournamentParticipation_GetByYear](
	@Year NCHAR(4)
)
AS
BEGIN
	SET NOCOUNT ON;

	-- Validate @Year
	IF @Year IS NULL OR LEN(@Year) != 4
		BEGIN
			THROW 50037, 'The Year parameter must be a 4-character string.', 1;
		END

	SELECT [GolferId], [TournamentId], [Year]
	FROM [dbo].[TournamentParticipation]
	WHERE [Year] = @Year;
END;
GO

CREATE PROCEDURE [dbo].[usp_TournamentParticipation_Delete](
	@GolferId INT,
	@TournamentId INT,
	@Year NCHAR(4),
	@RowCount INT OUTPUT
)
AS
BEGIN
	SET NOCOUNT ON;

	-- Validate @GolferId
	IF @GolferId IS NULL OR @GolferId <= 0
		BEGIN
			THROW 50038, 'The GolferId parameter must have a positive value.', 1;
		END

	-- Validate @TournamentId
	IF @TournamentId IS NULL OR @TournamentId <= 0
		BEGIN
			THROW 50039, 'The TournamentId parameter must have a positive value.', 1;
		END

	-- Validate @Year
	IF @Year IS NULL OR LEN(@Year) != 4
		BEGIN
			THROW 50040, 'The Year parameter must be a 4-character string.', 1;
		END

	-- Check if the record exists before attempting to delete
	IF EXISTS (SELECT 1
			   FROM [dbo].[TournamentParticipation]
			   WHERE [GolferId] = @GolferId
				 AND [TournamentId] = @TournamentId
				 AND [Year] = @Year)
		BEGIN
			BEGIN TRY
				BEGIN TRANSACTION;

				DELETE
				FROM [dbo].[TournamentParticipation]
				WHERE [GolferId] = @GolferId
				  AND [TournamentId] = @TournamentId
				  AND [Year] = @Year;

				SET @RowCount = @@ROWCOUNT;

				COMMIT TRANSACTION;
			END TRY
			BEGIN CATCH
				IF @@TRANCOUNT > 0
					ROLLBACK TRANSACTION;
				THROW;
			END CATCH;
		END
	ELSE
		BEGIN
			SET @RowCount = 0;
		END;
END;
GO
