CREATE PROCEDURE [dbo].[usp_Golfer_Insert](
	@FirstName NVARCHAR(256),
	@LastName NVARCHAR(256),
	@Email NVARCHAR(256),
	@JoinDate DATE,
	@Handicap INT = NULL,
	@GolferId INT OUTPUT
)
AS
BEGIN
	SET NOCOUNT ON;

	-- Validate @FirstName
	IF @FirstName IS NULL OR LEN(@FirstName) <= 0
		BEGIN
			THROW 50000, 'The FirstName parameter must have a value.', 1;
		END;

	-- Validate @LastName
	IF @LastName IS NULL OR LEN(@LastName) <= 0
		BEGIN
			THROW 50001, 'The LastName parameter must have a value.', 1;
		END;

	-- Validate @Email
	IF @Email IS NULL OR LEN(@Email) <= 0
		BEGIN
			THROW 50002, 'The Email parameter must have a value.', 1;
		END;

	IF EXISTS (SELECT 1 FROM [dbo].[Golfer] WHERE [Email] = @Email)
		BEGIN
			THROW 50003, 'Email already exists in the database.', 1;
		END;

	-- Validate @JoinDate
	IF @JoinDate IS NULL OR @JoinDate <= CONVERT(DATE, '0001-01-01') OR @JoinDate > CONVERT(DATE, GETDATE())
		BEGIN
			THROW 50004, 'The JoinDate parameter must not be null and must be greater than January 1, 0001, and less than the current date.', 1;
		END;

	-- Validate @Handicap
	IF @Handicap IS NOT NULL AND (@Handicap < 0 OR @Handicap > 54)
		BEGIN
			THROW 50005, 'The Handicap parameter must be between 0 and 54.', 1;
		END;

	BEGIN TRY
		BEGIN TRANSACTION;

		INSERT INTO [dbo].[Golfer] ([FirstName], [LastName], [Email], [JoinDate], [Handicap])
		VALUES (@FirstName, @LastName, @Email, @JoinDate, @Handicap);

		SELECT @GolferId = SCOPE_IDENTITY();

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

	-- Validate @GolferId
	IF @GolferId IS NULL OR @GolferId <= 0
		BEGIN
			THROW 50006, 'The GolferId parameter must have a positive value.', 1;
		END;

	SELECT [GolferId], [FirstName], [LastName], [Email], [JoinDate], [Handicap]
	FROM [dbo].[Golfer]
	WHERE [GolferId] = @GolferId;
END;
GO

CREATE PROCEDURE [dbo].[usp_Golfer_GetGolferByEmail](
	@Email NVARCHAR(256)
)
AS
BEGIN
	SET NOCOUNT ON;

	-- Validate @Email
	IF @Email IS NULL OR LEN(@Email) <= 0
		BEGIN
			THROW 50007, 'The Email parameter must have a value.', 1;
		END;

	SELECT [GolferId], [FirstName], [LastName], [Email], [JoinDate], [Handicap]
	FROM [dbo].[Golfer]
	WHERE [Email] = @Email;
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

	-- Validate @GolferId
	IF @GolferId IS NULL OR @GolferId <= 0
		BEGIN
			THROW 50008, 'The GolferId parameter must have a positive value.', 1;
		END;

	-- Validate @FirstName
	IF @FirstName IS NULL OR LEN(@FirstName) <= 0
		BEGIN
			THROW 50009, 'The FirstName parameter must have a value.', 1;
		END;

	-- Validate @LastName
	IF @LastName IS NULL OR LEN(@LastName) <= 0
		BEGIN
			THROW 50010, 'The LastName parameter must have a value.', 1;
		END;

	-- Validate @Email
	IF @Email IS NULL OR LEN(@Email) <= 0
		BEGIN
			THROW 50011, 'The Email parameter must have a value.', 1;
		END;

	IF EXISTS (SELECT 1 FROM [dbo].[Golfer] WHERE [Email] = @Email AND [GolferId] != @GolferId)
		BEGIN
			THROW 50012, 'Email already exists in the database.', 1;
		END;

	-- Validate @JoinDate
	IF @JoinDate IS NULL OR @JoinDate <= CONVERT(DATE, '0001-01-01') OR @JoinDate > CONVERT(DATE, GETDATE())
		BEGIN
			THROW 50013, 'The JoinDate parameter must not be null and must be greater than January 1, 0001, and less than the current date.', 1;
		END;

	-- Validate @Handicap
	IF @Handicap IS NOT NULL AND (@Handicap < 0 OR @Handicap > 54)
		BEGIN
			THROW 50014, 'The Handicap parameter must be between 0 and 54.', 1;
		END;

	IF EXISTS (SELECT 1 FROM [dbo].[Golfer] WHERE [GolferId] = @GolferId)
		BEGIN
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
		END;
	ELSE
		BEGIN
			THROW 50015, 'GolferId does not exist in the database.', 1;
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

	-- Validate @GolferId
	IF @GolferId IS NULL OR @GolferId <= 0
		BEGIN
			THROW 50016, 'The GolferId parameter must have a positive value.', 1;
		END;

	IF EXISTS (SELECT 1 FROM [dbo].[Golfer] WHERE [GolferId] = @GolferId)
		BEGIN
			BEGIN TRY
				BEGIN TRANSACTION;

				DELETE FROM [dbo].[Golfer]
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
