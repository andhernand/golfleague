ALTER PROCEDURE [dbo].[usp_Golfer_Delete](
	@GolferId INT,
	@RowCount INT OUTPUT
)
AS
BEGIN
	SET NOCOUNT ON;

	-- Validate @GolferId
	IF @GolferId IS NULL OR @GolferId <= 0
		BEGIN
			THROW 50016, 'The GolferId parameter must have a positive value.', 1;
		END;

	IF EXISTS (SELECT 1 FROM [dbo].[Golfer] WHERE [GolferId] = @GolferId)
		BEGIN
			BEGIN TRY
				BEGIN TRANSACTION;

				DELETE
				FROM [dbo].[TournamentParticipation]
				WHERE [GolferId] = @GolferId;

				DELETE
				FROM [dbo].[Golfer]
				WHERE [GolferId] = @GolferId;

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
