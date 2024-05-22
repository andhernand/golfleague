CREATE PROCEDURE [dbo].[usp_Tournament_Insert](
	@Name NVARCHAR(256),
	@Format NVARCHAR(256)
)
AS
BEGIN
	SET NOCOUNT ON;

	IF @Name IS NULL OR @Format IS NULL
		BEGIN
			RAISERROR ('Name and Format must have values.', 16, 1);
		END;

	BEGIN TRY
		BEGIN TRANSACTION;

		INSERT INTO [dbo].[Tournament] ([Name], [Format])
		OUTPUT inserted.TournamentId
		VALUES (@Name, @Format);

		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;
		THROW;
	END CATCH;
END;
GO

CREATE PROCEDURE [dbo].[usp_Tournament_GetTournamentById](
	@TournamentId INT
)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [TournamentId], [Name], [Format]
	FROM [dbo].[Tournament]
	WHERE [TournamentId] = @TournamentId;
END;
GO

CREATE PROCEDURE [dbo].[usp_Tournament_GetAll]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [TournamentId], [Name], [Format]
	FROM [dbo].[Tournament];
END;
GO

CREATE PROCEDURE [dbo].[usp_Tournament_Update](
	@TournamentId INT,
	@Name NVARCHAR(256),
	@Format NVARCHAR(256)
)
AS
BEGIN
	SET NOCOUNT ON;

	IF @Name IS NULL OR @Format IS NULL
		BEGIN
			RAISERROR ('Name and Format must have values.', 16, 1);
		END;

	IF EXISTS (SELECT 1 FROM [dbo].[Tournament] WHERE [TournamentId] = @TournamentId)
		BEGIN
			BEGIN TRY
				BEGIN TRANSACTION;

				UPDATE [dbo].[Tournament]
				SET [Name]   = @Name,
					[Format] = @Format
				WHERE [TournamentId] = @TournamentId;

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
			RAISERROR ('TournamentId does not exist in the database.', 16, 2);
		END;
END;
GO

CREATE PROCEDURE [dbo].[usp_Tournament_Delete](
	@TournamentId INT,
	@RowCount INT OUTPUT
)
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (SELECT 1 FROM [dbo].[Tournament] WHERE [TournamentId] = @TournamentId)
		BEGIN
			BEGIN TRY
				BEGIN TRANSACTION;

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
