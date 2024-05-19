CREATE PROCEDURE [dbo].[usp_TournamentParticipation_GetAll]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT
		tp.TournamentID,
		tp.MemberID,
		tp.Year
	FROM
		[dbo].[TournamentParticipation] tp;
END
GO

CREATE PROCEDURE [dbo].[usp_TournamentParticipation_GetByTournamentId]
	@tournamentId INT
AS
BEGIN
	SET NOCOUNT ON;

	IF (@tournamentId <= 0 OR @tournamentId IS NULL)
		BEGIN
			RAISERROR ('The parameter @tournamentId for procedure [dbo].[usp_Tournament_GetByTournamentId] may not be NULL.', 16, 1);
		END

	SELECT
		tp.TournamentID,
		tp.MemberID,
		tp.Year
	FROM
		[dbo].[TournamentParticipation] tp
	WHERE
		tp.TournamentID = @tournamentId;
END
GO

CREATE PROCEDURE [dbo].[usp_TournamentParticipation_GetByMemberId]
	@memberId INT
AS
BEGIN
	SET NOCOUNT ON;

	IF (@memberId <= 0 OR @memberId IS NULL)
		BEGIN
			RAISERROR ('The parameter @memberId for procedure [dbo].[usp_TournamentParticipation_GetByMemberId] may not be NULL.', 16, 1);
		END

	SELECT
		tp.TournamentID,
		tp.MemberID,
		tp.Year
	FROM
		[dbo].[TournamentParticipation] tp
	WHERE
		tp.MemberID = @memberId;
END
GO

CREATE PROCEDURE [dbo].[usp_TournamentParticipation_GetByYear]
	@year NCHAR(4)
AS
BEGIN
	SET NOCOUNT ON;

	IF (@year IS NULL OR LEN(@year) = 0)
		BEGIN
			RAISERROR ('The parameter @year for procedure [dbo].[usp_TournamentParticipation_GetByYear] may not be NULL.', 16, 1);
		END

	SELECT
		tp.TournamentID,
		tp.MemberID,
		tp.Year
	FROM
		[dbo].[TournamentParticipation] tp
	WHERE
		tp.Year = @year;
END
GO

CREATE PROCEDURE [dbo].[usp_TournamentParticipation_Insert]
	@tournamentId INT,
	@memberId INT,
	@year NCHAR(4)
AS
BEGIN
	SET NOCOUNT ON;

	IF (@tournamentId <= 0 OR @tournamentId IS NULL)
		BEGIN
			RAISERROR ('The parameter @tournamentId for procedure [dbo].[usp_TournamentParticipation_Insert] may not be NULL.', 16, 1);
		END

	IF (@memberId <= 0 OR @memberId IS NULL)
		BEGIN
			RAISERROR ('The parameter @memberId for procedure [dbo].[usp_TournamentParticipation_Insert] may not be NULL.', 16, 1);
		END

	IF (@year IS NULL OR LEN(@year) = 0)
		BEGIN
			RAISERROR ('The parameter @year for procedure [dbo].[usp_TournamentParticipation_Insert] may not be NULL.', 16, 1);
		END

	INSERT INTO [dbo].[TournamentParticipation] (MemberID, TournamentID, Year)
	VALUES (@memberId, @tournamentId, @year);
END
GO

CREATE PROCEDURE [dbo].[usp_TournamentParticipation_Delete]
	@tournamentId INT,
	@memberId INT,
	@year NCHAR(4)
AS
BEGIN
	SET NOCOUNT ON;

	IF (@tournamentId <= 0 OR @tournamentId IS NULL)
		BEGIN
			RAISERROR ('The parameter @tournamentId for procedure [dbo].[usp_TournamentParticipation_Delete] may not be NULL.', 16, 1);
		END

	IF (@memberId <= 0 OR @memberId IS NULL)
		BEGIN
			RAISERROR ('The parameter @memberId for procedure [dbo].[usp_TournamentParticipation_Delete] may not be NULL.', 16, 1);
		END

	IF (@year IS NULL OR LEN(@year) = 0)
		BEGIN
			RAISERROR ('The parameter @year for procedure [dbo].[usp_TournamentParticipation_Delete] may not be NULL.', 16, 1);
		END

	DELETE
	FROM
		[dbo].[TournamentParticipation]
	WHERE
		TournamentID = @tournamentId
		AND MemberID = @memberId
		AND Year = @year;
END
GO
