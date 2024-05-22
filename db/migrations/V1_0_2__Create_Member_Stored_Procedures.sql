CREATE PROCEDURE [dbo].[usp_Golfer_Insert](
	@FirstName NVARCHAR(256),
	@LastName NVARCHAR(256),
	@Email NVARCHAR(256),
	@JoinDate DATE,
	@Handicap TINYINT = NULL
)
AS
BEGIN
	SET NOCOUNT ON;

	IF @FirstName IS NULL OR @LastName IS NULL OR @Email IS NULL OR @JoinDate IS NULL
		BEGIN
			RAISERROR ('All input parameters must have values.', 16, 1);
		END;

	IF EXISTS (SELECT 1 FROM [dbo].[Golfer] WHERE [Email] = @Email)
		BEGIN
			RAISERROR ('Email already exists in the database.', 16, 2);
		END;

	IF @Handicap IS NOT NULL AND @Handicap < 0
		BEGIN
			RAISERROR ('Handicap must be a non-negative value.', 16, 3);
		END;

	BEGIN TRY
		BEGIN TRANSACTION;

		INSERT INTO [dbo].[Golfer] ([FirstName], [LastName], [Email], [JoinDate], [Handicap])
		OUTPUT inserted.GolferId
		VALUES (@FirstName, @LastName, @Email, @JoinDate, @Handicap);

		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;
		THROW;
	END CATCH;
END;
GO

CREATE PROCEDURE [dbo].[usp_Golfer_GetGolferById](
	@GolferId INT
)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [GOlferId], [FirstName], [LastName], [Email], [JoinDate], [Handicap]
	FROM [dbo].[Golfer]
	WHERE [GolferId] = @GolferId;
END;
GO

CREATE PROCEDURE [dbo].[usp_Golfer_GetAll]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [GolferId], [FirstName], [LastName], [Email], [JoinDate], [Handicap]
	FROM [dbo].[Golfer];
END;
GO

CREATE PROCEDURE [dbo].[usp_Golfer_Update](
	@GolferId INT,
	@FirstName NVARCHAR(256),
	@LastName NVARCHAR(256),
	@Email NVARCHAR(256),
	@JoinDate DATE,
	@Handicap TINYINT = NULL
)
AS
BEGIN
	SET NOCOUNT ON;

	IF @FirstName IS NULL OR @LastName IS NULL OR @Email IS NULL OR @JoinDate IS NULL
		BEGIN
			RAISERROR ('All input parameters must have values.', 16, 1);
		END;

	IF EXISTS (SELECT 1 FROM [dbo].[Golfer] WHERE [GolferId] = @GolferId)
		BEGIN
			IF EXISTS (SELECT 1 FROM [dbo].[Golfer] WHERE [Email] = @Email AND [GolferId] != @GolferId)
				BEGIN
					RAISERROR ('Email already exists in the database.', 16, 2);
				END;

			IF @Handicap IS NOT NULL AND @Handicap < 0
				BEGIN
					RAISERROR ('Handicap must be a non-negative value.', 16, 3);
				END;

			BEGIN TRY
				BEGIN TRANSACTION;

				UPDATE [dbo].[Golfer]
				SET [FirstName] = @FirstName,
					[LastName]  = @LastName,
					[Email]     = @Email,
					[JoinDate]  = @JoinDate,
					[Handicap]  = @Handicap
				WHERE [GolferId] = @GolferId;

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
			RAISERROR ('GolferId does not exist in the database.', 16, 4);
		END;
END;
GO

CREATE PROCEDURE [dbo].[usp_Golfer_Delete](
	@GolferId INT,
	@RowCount INT OUTPUT
)
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (SELECT 1 FROM [dbo].[Golfer] WHERE [GolferId] = @GolferId)
		BEGIN
			BEGIN TRY
				BEGIN TRANSACTION;

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