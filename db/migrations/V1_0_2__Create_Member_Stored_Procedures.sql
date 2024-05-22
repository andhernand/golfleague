CREATE PROCEDURE [dbo].[usp_Member_Insert](
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

	IF EXISTS (SELECT 1 FROM [dbo].[Member] WHERE [Email] = @Email)
		BEGIN
			RAISERROR ('Email already exists in the database.', 16, 2);
		END;

	IF @Handicap IS NOT NULL AND @Handicap < 0
		BEGIN
			RAISERROR ('Handicap must be a non-negative value.', 16, 3);
		END;

	BEGIN TRY
		BEGIN TRANSACTION;

		INSERT INTO [dbo].[Member] ([FirstName], [LastName], [Email], [JoinDate], [Handicap])
		OUTPUT inserted.MemberId
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

CREATE PROCEDURE [dbo].[usp_Member_GetMemberById](
	@MemberId INT
)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [MemberId], [FirstName], [LastName], [Email], [JoinDate], [Handicap]
	FROM [dbo].[Member]
	WHERE [MemberId] = @MemberId;
END;
GO

CREATE PROCEDURE [dbo].[usp_Member_GetAll]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [MemberId], [FirstName], [LastName], [Email], [JoinDate], [Handicap]
	FROM [dbo].[Member];
END;
GO

CREATE PROCEDURE [dbo].[usp_Member_Update](
	@MemberId INT,
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

	IF EXISTS (SELECT 1 FROM [dbo].[Member] WHERE [MemberId] = @MemberId)
		BEGIN
			IF EXISTS (SELECT 1 FROM [dbo].[Member] WHERE [Email] = @Email AND [MemberId] != @MemberId)
				BEGIN
					RAISERROR ('Email already exists in the database.', 16, 2);
				END;

			IF @Handicap IS NOT NULL AND @Handicap < 0
				BEGIN
					RAISERROR ('Handicap must be a non-negative value.', 16, 3);
				END;

			BEGIN TRY
				BEGIN TRANSACTION;

				UPDATE [dbo].[Member]
				SET [FirstName] = @FirstName,
					[LastName]  = @LastName,
					[Email]     = @Email,
					[JoinDate]  = @JoinDate,
					[Handicap]  = @Handicap
				WHERE [MemberId] = @MemberId;

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
			RAISERROR ('MemberId does not exist in the database.', 16, 4);
		END;
END;
GO

CREATE PROCEDURE [dbo].[usp_Member_Delete](
	@MemberId INT,
	@RowCount INT OUTPUT
)
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (SELECT 1 FROM [dbo].[Member] WHERE [MemberId] = @MemberId)
		BEGIN
			BEGIN TRY
				BEGIN TRANSACTION;

				DELETE
				FROM [dbo].[Member]
				WHERE [MemberId] = @MemberId;

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