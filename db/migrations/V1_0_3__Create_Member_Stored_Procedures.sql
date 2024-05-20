CREATE PROCEDURE [dbo].[usp_Member_GetAll]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
		m.MemberID,
		m.LastName,
		m.FirstName,
		m.MemberTypeID,
		m.Phone,
		m.Handicap,
		m.JoinDate,
		m.Coach,
		m.Gender
	FROM
		[dbo].[Member] m;
END
GO

CREATE PROCEDURE [dbo].[usp_Member_GetByMemberId]
	@memberId INT
AS
BEGIN
	SET NOCOUNT ON;

	IF (@memberId <= 0 OR @memberId IS NULL)
		BEGIN
			RAISERROR ('The parameter @memberId for procedure [dbo].[usp_Member_GetByMemberId] may not be NULL.', 16, 1);
		END

	SELECT
		m.MemberID,
		m.LastName,
		m.FirstName,
		m.MemberTypeID,
		m.Phone,
		m.Handicap,
		m.JoinDate,
		m.Coach,
		m.Gender
	FROM
		[dbo].[Member] m
	WHERE
		m.MemberID = @memberId;
END
GO

CREATE PROCEDURE [dbo].[usp_Member_Insert]
	@lastName NVARCHAR(256),
	@firstName NVARCHAR(256),
	@memberTypeID INT,
	@phone NVARCHAR(256) = NULL,
	@handicap TINYINT = NULL,
	@joinDate DATE,
	@coach INT = NULL,
	@gender NCHAR(1)
AS
BEGIN
	SET NOCOUNT ON;

	IF (@lastName IS NULL OR LEN(@lastName) = 0)
		BEGIN
			RAISERROR ('The parameter @lastName for procedure [dbo].[usp_Member_Insert] may not be NULL.', 16, 1);
		END

	IF (@firstName IS NULL OR LEN(@firstName) = 0)
		BEGIN
			RAISERROR ('The parameter @firstName for procedure [dbo].[usp_Member_Insert] may not be NULL.', 16, 1);
		END

	IF (@memberTypeID <= 0 OR @memberTypeID IS NULL)
		BEGIN
			RAISERROR ('The parameter @memberTypeID for procedure [dbo].[usp_Member_Insert] may not be NULL.', 16, 1);
		END

	IF (@joinDate IS NULL)
		BEGIN
			RAISERROR ('The parameter @joinDate for procedure [dbo].[usp_Member_Insert] may not be NULL.', 16, 1);
		END

	IF (@gender IS NULL OR LEN(@gender) = 0)
		BEGIN
			RAISERROR ('The parameter @gender for procedure [dbo].[usp_Member_Insert] may not be NULL.', 16, 1);
		END

	INSERT INTO [dbo].[Member] (LastName, FirstName, MemberTypeID, Phone, Handicap, JoinDate, Coach, Gender)
	VALUES (@lastName, @firstName, @memberTypeID, @phone, @handicap, @joinDate, @coach, @gender);

	SELECT SCOPE_IDENTITY() AS memberId;
END
GO

CREATE PROCEDURE [dbo].[usp_Member_Update]
	@memberID INT,
	@lastName NVARCHAR(256),
	@firstName NVARCHAR(256),
	@memberTypeID INT,
	@phone NVARCHAR(256),
	@handicap TINYINT,
	@joinDate DATE,
	@coach INT,
	@gender NCHAR(1)
AS
BEGIN
	SET NOCOUNT ON;

	IF (@lastName IS NULL OR LEN(@lastName) = 0)
		BEGIN
			RAISERROR ('The parameter @lastName for procedure [dbo].[usp_Member_Update] may not be NULL.', 16, 1);
		END

	IF (@firstName IS NULL OR LEN(@firstName) = 0)
		BEGIN
			RAISERROR ('The parameter @firstName for procedure [dbo].[usp_Member_Update] may not be NULL.', 16, 1);
		END

	IF (@memberTypeID <= 0 OR @memberTypeID IS NULL)
		BEGIN
			RAISERROR ('The parameter @memberTypeID for procedure [dbo].[usp_Member_Update] may not be NULL.', 16, 1);
		END

	IF (@joinDate IS NULL)
		BEGIN
			RAISERROR ('The parameter @joinDate for procedure [dbo].[usp_Member_Update] may not be NULL.', 16, 1);
		END

	IF (@gender IS NULL OR LEN(@gender) = 0)
		BEGIN
			RAISERROR ('The parameter @gender for procedure [dbo].[usp_Member_Update] may not be NULL.', 16, 1);
		END

	UPDATE
		[dbo].[Member]
	SET
		LastName = @lastName,
		FirstName = @firstName,
		MemberTypeID = @memberTypeID,
		Phone = @phone,
		Handicap = @handicap,
		JoinDate = @joinDate,
		Coach = @coach,
		Gender = @gender
	WHERE
		MemberID = @memberID;
END
GO

CREATE PROCEDURE [dbo].[usp_Member_Delete]
	@memberId INT
AS
BEGIN
	SET NOCOUNT ON;

	IF (@memberId <= 0 OR @memberId IS NULL)
		BEGIN
			RAISERROR ('The parameter @memberId for procedure [dbo].[usp_Member_Delete] may not be NULL.', 16, 1);
		END

	DELETE
	FROM
		[dbo].[Member]
	WHERE
		MemberID = @memberId;
END
GO
