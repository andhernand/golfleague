CREATE PROCEDURE [dbo].[usp_MemberType_GetByName]
	@name NVARCHAR(256)
AS
BEGIN
	SET NOCOUNT ON;

	IF (@name IS NULL OR LEN(@name) = 0)
		BEGIN
			RAISERROR('The parameter @name for procedure [dbo].[usp_MemberType_GetByName] may not be NULL.', 16, 1);
		END

	SELECT mt.MemberTypeID,
		   mt.Name,
		   mt.Fee
	FROM [dbo].[MemberType] mt
	WHERE mt.Name = @name;
END
GO