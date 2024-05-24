CREATE PROCEDURE [dbo].[usp_Tournament_Insert](
	@Name NVARCHAR(256),
	@Format NVARCHAR(256),
	@TournamentId INT OUTPUT
)
AS
BEGIN
	SET NOCOUNT ON;

	IF @Name IS NULL OR LEN(@Name) <= 0
		BEGIN
			THROW 50000, 'The Name parameter must have a value.', 1;
		END

	IF @Format IS NULL OR LEN(@Format) <= 0
		BEGIN
			THROW 50001, 'The Format parameter must have a value.', 1;
		END

	BEGIN TRY
		BEGIN TRANSACTION;

		INSERT INTO [dbo].[Tournament] ([Name], [Format])
		OUTPUT inserted.TournamentId
		VALUES (@Name, @Format);

		SELECT @TournamentId = SCOPE_IDENTITY();

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

	IF @TournamentId IS NULL OR @TournamentId <= 0
		BEGIN
			THROW 50002, 'The TournamentId parameter must have a value.', 1;
		END

	SELECT [TournamentId], [Name], [Format]
	FROM [dbo].[Tournament]
	WHERE [TournamentId] = @TournamentId;
END;
GO

CREATE PROCEDURE [dbo].[usp_Tournament_GetTournamentByNameAndFormat](
	@Name NVARCHAR(256),
	@Format NVARCHAR(256)
)
AS
BEGIN
	SET NOCOUNT ON;

	IF @Name IS NULL OR LEN(@Name) <= 0
		BEGIN
			THROW 50003, 'The Name parameter must have a value.', 1;
		END

	IF @Format IS NULL OR LEN(@Format) <= 0
		BEGIN
			THROW 50004, 'The Format parameter must have a value.', 1;
		END

	SELECT [TournamentId], [Name], [Format]
	FROM [dbo].[Tournament]
	WHERE [Name] = @Name
	  AND [Format] = @Format;
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

	IF @TournamentId IS NULL OR @TournamentId <= 0
		BEGIN
			THROW 50005, 'The TournamentId parameter must have a value.', 1;
		END

	IF @Name IS NULL OR LEN(@Name) <= 0
		BEGIN
			THROW 50006, 'The Name parameter must have a value.', 1;
		END

	IF @Format IS NULL OR LEN(@Format) <= 0
		BEGIN
			THROW 50007, 'The Format parameter must have a value.', 1;
		END

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
			THROW 50008, 'TournamentId does not exist in the database.', 1;
		END;
END;
GO

CREATE PROCEDURE [dbo].[usp_Tournament_Delete](
	@TournamentId INT,
	@RowCount INT = 0 OUTPUT
)
AS
BEGIN
	SET NOCOUNT ON;

	IF @TournamentId IS NULL OR @TournamentId <= 0
		BEGIN
			THROW 50009, 'The TournamentId parameter must have a value.', 1;
		END

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
