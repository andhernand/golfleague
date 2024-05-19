CREATE PROCEDURE [dbo].[usp_MemberType_GetAll]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
		mt.MemberTypeID,
		mt.Name,
		mt.Fee
	FROM
		[dbo].[MemberType] mt;
END
GO

CREATE PROCEDURE [dbo].[usp_MemberType_Get]
	@memberTypeId INT
AS
BEGIN
	SET NOCOUNT ON;

	IF (@memberTypeId <= 0 OR @memberTypeId IS NULL)
		BEGIN
			RAISERROR('The parameter @memberTypeId for procedure [dbo].[usp_MemberType_Get] may not be NULL.', 16, 1);
		END

	SELECT
		mt.MemberTypeID,
		mt.Name,
		mt.Fee
	FROM
		[dbo].[MemberType] mt
	WHERE
		mt.MemberTypeID = @memberTypeId;
END
GO

CREATE PROCEDURE [dbo].[usp_MemberType_Insert]
	@name NVARCHAR(256),
	@fee DECIMAL(19,2)
AS
BEGIN
	SET NOCOUNT ON;

	IF (@name IS NULL OR LEN(@name) = 0)
		BEGIN
			RAISERROR('The parameter @name for procedure [dbo].[usp_MemberType_Insert] may not be NULL.', 16, 1);
		END

	INSERT INTO [dbo].[MemberType] (Name, Fee)
	VALUES (@name, @fee);

	SELECT SCOPE_IDENTITY() AS memberTypeId;
END
GO

CREATE PROCEDURE [dbo].[usp_MemberType_Update]
	@memberTypeId INT,
	@name NVARCHAR(256),
	@fee DECIMAL(19,2)
AS
BEGIN
	SET NOCOUNT ON;

	IF (@memberTypeId <= 0 OR @memberTypeId IS NULL)
		BEGIN
			RAISERROR('The parameter @memberTypeId for procedure [dbo].[usp_MemberType_Update] cannot be NULL.', 16, 1);
		END

	IF (@name IS NULL OR LEN(@name) = 0)
		BEGIN
			RAISERROR('The parameter @name for procedure [dbo].[usp_MemberType_Update] cannot be NULL.', 16, 1);
		END

	UPDATE
		[dbo].[MemberType]
	SET
		Name = @name,
		Fee = @fee
	WHERE
		MemberTypeID = @memberTypeId;
END
GO

CREATE PROCEDURE [dbo].[usp_MemberType_Delete]
	@memberTypeId INT
AS
BEGIN
	SET NOCOUNT ON;

	IF (@memberTypeId <= 0 OR @memberTypeId IS NULL)
		BEGIN
			RAISERROR('The parameter @memberTypeId for procedure [dbo].[usp_MemberType_Delete] may not be NULL.', 16, 1);
		END

	DELETE
	FROM
		[dbo].[MemberType]
	WHERE
		MemberTypeID = @memberTypeId;
END
GO
