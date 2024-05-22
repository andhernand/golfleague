CREATE TABLE [dbo].[Member]
(
	[MemberId]  INT IDENTITY (1, 1) NOT NULL,
	[FirstName] NVARCHAR(256)       NOT NULL,
	[LastName]  NVARCHAR(256)       NOT NULL,
	[Email]     NVARCHAR(256)       NOT NULL,
	[JoinDate]  DATE                NOT NULL,
	[Handicap]  TINYINT             NULL,
	CONSTRAINT [PK_Member_MemberId]
		PRIMARY KEY CLUSTERED ([MemberId] ASC),
	CONSTRAINT [AK_Member_Email]
		UNIQUE ([Email])
);
GO

CREATE TABLE [dbo].[Tournament]
(
	[TournamentId] INT IDENTITY (1, 1) NOT NULL,
	[Name]         NVARCHAR(256)       NOT NULL,
	[Format]       NVARCHAR(256)       NOT NULL,
	CONSTRAINT [PK_Tournament_TournamentId]
		PRIMARY KEY CLUSTERED ([TournamentId] ASC)
);
GO

CREATE TABLE [dbo].[TournamentParticipation]
(
	[MemberId]     INT      NOT NULL,
	[TournamentId] INT      NOT NULL,
	[Year]         NCHAR(4) NOT NULL,
	CONSTRAINT [PK_TournamentParticipation_MemberId_TournamentId_Year]
		PRIMARY KEY CLUSTERED ([MemberId] ASC, [TournamentId] ASC, [Year] ASC),
	CONSTRAINT [FK_Member_TournamentParticipation_MemberId]
		FOREIGN KEY ([MemberId]) REFERENCES [dbo].[Member] ([MemberId]),
	CONSTRAINT [FK_Tournament_TournamentParticipation_TournamentId]
		FOREIGN KEY ([TournamentId]) REFERENCES [dbo].[Tournament] ([TournamentId])
);
GO

ALTER TABLE [dbo].[Member]
	WITH CHECK ADD CONSTRAINT [CK_Member_Handicap] CHECK (([Handicap] IS NULL OR [Handicap] >= 0));
GO

ALTER TABLE [dbo].[Member]
	CHECK CONSTRAINT [CK_Member_Handicap];
GO
