ALTER PROCEDURE [dbo].[usp_Tournament_Delete](
	@TournamentId INT,
	@RowCount INT OUTPUT
)
AS
BEGIN
	SET NOCOUNT ON;

	-- Validate @TournamentId
	IF @TournamentId IS NULL OR @TournamentId <= 0
		BEGIN
			THROW 50026, 'The TournamentId parameter must have a positive value.', 1;
		END;

	IF EXISTS (SELECT 1 FROM [dbo].[Tournament] WHERE [TournamentId] = @TournamentId)
		BEGIN
			BEGIN TRY
				BEGIN TRANSACTION;

				DELETE
				FROM [dbo].[TournamentParticipation]
				WHERE [TournamentId] = @TournamentId;

				DELETE
				FROM [dbo].[Tournament]
				WHERE [TournamentId] = @TournamentId;

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
