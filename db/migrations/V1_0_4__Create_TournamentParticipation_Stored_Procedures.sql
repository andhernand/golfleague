CREATE PROCEDURE [dbo].[usp_TournamentParticipation_Insert](
	@MemberId INT,
	@TournamentId INT,
	@Year NCHAR(4)
)
AS
BEGIN
	SET NOCOUNT ON;

	IF @MemberId IS NULL OR @TournamentId IS NULL OR @Year IS NULL
		BEGIN
			RAISERROR ('MemberId, TournamentId, and Year must have values.', 16, 1);
		END;

	BEGIN TRY
		BEGIN TRANSACTION;

		INSERT INTO [dbo].[TournamentParticipation] ([MemberId], [TournamentId], [Year])
		VALUES (@MemberId, @TournamentId, @Year);

		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
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

	SELECT [MemberId], [TournamentId], [Year]
	FROM [dbo].[TournamentParticipation];
END;
GO

CREATE PROCEDURE [dbo].[usp_TournamentParticipation_GetByMemberId](
	@MemberId INT
)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [MemberId], [TournamentId], [Year]
	FROM [dbo].[TournamentParticipation]
	WHERE [MemberId] = @MemberId;
END;
GO

CREATE PROCEDURE [dbo].[usp_TournamentParticipation_GetByTournamentId](
	@TournamentId INT
)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [MemberId], [TournamentId], [Year]
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

	SELECT [MemberId], [TournamentId], [Year]
	FROM [dbo].[TournamentParticipation]
	WHERE [Year] = @Year;
END;
GO

CREATE PROCEDURE [dbo].[usp_TournamentParticipation_Delete](
	@MemberId INT,
	@TournamentId INT,
	@Year NCHAR(4),
	@RowCount INT OUTPUT
)
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (SELECT 1
			   FROM [dbo].[TournamentParticipation]
			   WHERE [MemberId] = @MemberId
				 AND [TournamentId] = @TournamentId
				 AND [Year] = @Year)
		BEGIN
			BEGIN TRY
				BEGIN TRANSACTION;

				DELETE
				FROM [dbo].[TournamentParticipation]
				WHERE [MemberId] = @MemberId
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
