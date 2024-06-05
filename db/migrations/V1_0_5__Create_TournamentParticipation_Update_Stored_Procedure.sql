CREATE PROCEDURE [dbo].[usp_TournamentParticipation_Update](
	@OriginalGolferId INT,
	@OriginalTournamentId INT,
	@OriginalYear NVARCHAR(4),
	@NewGolferId INT,
	@NewTournamentId INT,
	@NewYear NVARCHAR(4)
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

	BEGIN TRY
		BEGIN TRANSACTION;

		UPDATE [dbo].[TournamentParticipation]
		SET [GolferId]     = @NewGolferId,
			[TournamentId] = @NewGolferId,
			[Year]         = @NewYear
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
END
GO
