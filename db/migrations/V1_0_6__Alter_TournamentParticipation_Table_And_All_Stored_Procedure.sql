ALTER TABLE [dbo].[TournamentParticipation]
	ADD [Score] INT NULL;
GO

ALTER TABLE [dbo].[TournamentParticipation]
	WITH CHECK ADD CONSTRAINT [CK_TournamentParticipation_Score]
		CHECK (([Score] IS NULL OR ([Score] >= 50 AND [Score] <= 130)));
GO

ALTER TABLE [dbo].[TournamentParticipation]
	CHECK CONSTRAINT [CK_TournamentParticipation_Score];
GO

ALTER PROCEDURE [dbo].[usp_Golfer_GetGolferById](
	@GolferId INT
)
AS
BEGIN
	SET NOCOUNT ON;

	-- Validate @GolferId
	IF @GolferId IS NULL OR @GolferId <= 0
		BEGIN
			THROW 50006, 'The GolferId parameter must have a positive value.', 1;
		END;

	SELECT G.[GolferId],
		   G.[FirstName],
		   G.[LastName],
		   G.[Email],
		   G.[JoinDate],
		   G.[Handicap],
		   T.[TournamentId],
		   T.[Name],
		   T.[Format],
		   TP.[Year],
		   TP.[Score]
	FROM [dbo].[Golfer] G
			 LEFT JOIN [dbo].[TournamentParticipation] TP ON G.[GolferId] = TP.[GolferId]
			 LEFT JOIN [dbo].[Tournament] T ON TP.[TournamentId] = T.[TournamentId]
	WHERE G.[GolferId] = @GolferId
	ORDER BY G.[GolferId],
			 TP.[Year],
			 T.[TournamentId];
END;
GO

ALTER PROCEDURE [dbo].[usp_Golfer_GetAll]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT G.[GolferId],
		   G.[FirstName],
		   G.[LastName],
		   G.[Email],
		   G.[JoinDate],
		   G.[Handicap],
		   T.[TournamentId],
		   T.[Name],
		   T.[Format],
		   TP.[Year],
		   TP.[Score]
	FROM [dbo].[Golfer] G
			 LEFT JOIN [dbo].[TournamentParticipation] TP ON G.[GolferId] = TP.[GolferId]
			 LEFT JOIN [dbo].[Tournament] T ON TP.[TournamentId] = T.[TournamentId]
	ORDER BY G.[GolferId],
			 TP.[Year],
			 T.[TournamentId];
END;
GO

ALTER PROCEDURE [dbo].[usp_Tournament_GetTournamentById](
	@TournamentId INT
)
AS
BEGIN
	SET NOCOUNT ON;

	-- Validate @TournamentId
	IF @TournamentId IS NULL OR @TournamentId <= 0
		BEGIN
			THROW 50019, 'The TournamentId parameter must have a positive value.', 1;
		END;

	SELECT T.[TournamentId],
		   T.[Name],
		   T.[Format],
		   G.[GolferId],
		   G.[FirstName],
		   G.[LastName],
		   TP.[Year],
		   TP.[Score]
	FROM [dbo].[Tournament] T
			 LEFT JOIN [dbo].[TournamentParticipation] TP ON T.[TournamentId] = TP.[TournamentId]
			 LEFT JOIN [dbo].[Golfer] G ON TP.[GolferId] = G.[GolferId]
	WHERE T.[TournamentId] = @TournamentId
	ORDER BY T.[TournamentId],
			 TP.[Year],
			 G.[GolferId];
END;
GO

ALTER PROCEDURE [dbo].[usp_Tournament_GetAll]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT T.[TournamentId],
		   T.[Name],
		   T.[Format],
		   G.[GolferId],
		   G.[FirstName],
		   G.[LastName],
		   TP.[Year],
		   TP.[Score]
	FROM [dbo].[Tournament] T
			 LEFT JOIN [dbo].[TournamentParticipation] TP ON T.[TournamentId] = TP.[TournamentId]
			 LEFT JOIN [dbo].[Golfer] G ON TP.[GolferId] = G.[GolferId]
	ORDER BY T.[TournamentId],
			 TP.[Year],
			 G.[GolferId];
END;
GO

ALTER PROCEDURE [dbo].[usp_TournamentParticipation_Insert](
	@GolferId INT,
	@TournamentId INT,
	@Year NCHAR(4),
	@Score INT = NULL,
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

	-- Validate @Score
	IF @Score IS NOT NULL AND (@Score < 50 OR @Score > 130)
		BEGIN
			THROW 50005, 'The Score parameter must be between 50 and 130.', 1;
		END

	BEGIN TRY
		BEGIN TRANSACTION;

		INSERT INTO [dbo].[TournamentParticipation] ([GolferId], [TournamentId], [Year], [Score])
		VALUES (@GolferId, @TournamentId, @Year, @Score);

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

ALTER PROCEDURE [dbo].[usp_TournamentParticipation_GetById](
	@GolferId INT,
	@TournamentId INT,
	@Year NCHAR(4)
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
			THROW 50047, 'The Year parameter must be a 4-character string.', 1;
		END

	SELECT [GolferId], [TournamentId], [Year], [Score]
	FROM [dbo].[TournamentParticipation]
	WHERE [GolferId] = @GolferId
	  AND [TournamentId] = @TournamentId
	  AND [Year] = @Year;
END;
GO

ALTER PROCEDURE [dbo].[usp_TournamentParticipation_Update](
	@OriginalGolferId INT,
	@OriginalTournamentId INT,
	@OriginalYear NVARCHAR(4),
	@NewGolferId INT,
	@NewTournamentId INT,
	@NewYear NVARCHAR(4),
	@Score INT = NULL
)
AS
BEGIN
	SET NOCOUNT ON;

	-- Validate @OriginalGolferId
	IF @OriginalGolferId IS NULL OR @OriginalGolferId <= 0
		BEGIN
			THROW 50038, 'The OriginalGolferId parameter must have a positive value.', 1;
		END

	-- Validate @OriginalTournamentId
	IF @OriginalTournamentId IS NULL OR @OriginalTournamentId <= 0
		BEGIN
			THROW 50039, 'The OriginalTournamentId parameter must have a positive value.', 1;
		END

	-- Validate @OriginalYear
	IF @OriginalYear IS NULL OR LEN(@OriginalYear) != 4
		BEGIN
			THROW 50040, 'The OriginalYear parameter must be a 4-character string.', 1;
		END

	-- Validate Original TournamentParticipation
	IF NOT EXISTS (SELECT 1
				   FROM [dbo].[TournamentParticipation]
				   WHERE [GolferId] = @OriginalGolferId
					 AND [TournamentId] = @OriginalTournamentId
					 AND [Year] = @OriginalYear)
		BEGIN
			THROW 50041, 'The original TournamentParticipation does not exist in the database.', 1;
		END

	-- Validate @NewGolferId
	IF @NewGolferId IS NULL OR @NewGolferId <= 0
		BEGIN
			THROW 50042, 'The NewGolferId parameter must have a positive value.', 1;
		END

	-- Validate Golfer exists
	IF NOT EXISTS (SELECT 1 FROM [dbo].[Golfer] WHERE [GolferId] = @NewGolferId)
		BEGIN
			THROW 50043, 'The specified NewGolferId does not exist in the database.', 1;
		END

	-- Validate @NewTournamentId
	IF @NewTournamentId IS NULL OR @NewTournamentId <= 0
		BEGIN
			THROW 50044, 'The NewTournamentId parameter must have a positive value.', 1;
		END

	-- Validate Tournament exists
	IF NOT EXISTS (SELECT 1 FROM [dbo].[Tournament] WHERE [TournamentId] = @NewTournamentId)
		BEGIN
			THROW 50045, 'The specified NewTournamentId does not exist in the database.', 1;
		END

	-- Validate @NewYear
	IF @NewYear IS NULL OR LEN(@NewYear) != 4
		BEGIN
			THROW 50046, 'The NewYear parameter must be a 4-character string.', 1;
		END

	-- Validate @Score
	IF @Score IS NOT NULL AND (@Score < 50 OR @Score > 130)
		BEGIN
			THROW 50048, 'The Score parameter must be between 50 and 130.', 1;
		END

	BEGIN TRY
		BEGIN TRANSACTION;

		UPDATE [dbo].[TournamentParticipation]
		SET [GolferId]     = @NewGolferId,
			[TournamentId] = @NewGolferId,
			[Year]         = @NewYear,
			[Score]        = @Score
		WHERE [GolferId] = @OriginalGolferId
		  AND [TournamentId] = @OriginalTournamentId
		  AND [Year] = @OriginalYear;

		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;
		THROW;
	END CATCH
END;
GO
