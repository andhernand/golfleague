CREATE TABLE [dbo].[Golfer]
(
	[GolferId]  INT IDENTITY (1, 1) NOT NULL,
	[FirstName] NVARCHAR(256)       NOT NULL,
	[LastName]  NVARCHAR(256)       NOT NULL,
	[Email]     NVARCHAR(256)       NOT NULL,
	[JoinDate]  DATE                NOT NULL,
	[Handicap]  INT                 NULL,
	CONSTRAINT [PK_Golfer_GolferId]
		PRIMARY KEY CLUSTERED ([GolferId] ASC),
	CONSTRAINT [AK_Golfer_Email]
		UNIQUE ([Email])
);
GO

ALTER TABLE [dbo].[Golfer]
	WITH CHECK ADD CONSTRAINT [CK_Golfer_Handicap]
		CHECK (([Handicap] IS NULL OR ([Handicap] >= 0 AND [Handicap] <= 54)));
GO

ALTER TABLE [dbo].[Golfer]
	CHECK CONSTRAINT [CK_Golfer_Handicap];
GO

CREATE TABLE [dbo].[Tournament]
(
	[TournamentId] INT IDENTITY (1, 1) NOT NULL,
	[Name]         NVARCHAR(256)       NOT NULL,
	[Format]       NVARCHAR(256)       NOT NULL,
	CONSTRAINT [PK_Tournament_TournamentId]
		PRIMARY KEY CLUSTERED ([TournamentId] ASC),
	CONSTRAINT [AK_Tournament_Name_Format]
		UNIQUE ([Name], [Format])
);
GO

CREATE TABLE [dbo].[TournamentParticipation]
(
	[GolferId]     INT      NOT NULL,
	[TournamentId] INT      NOT NULL,
	[Year]         NCHAR(4) NOT NULL,
	CONSTRAINT [PK_TournamentParticipation_GolferId_TournamentId_Year]
		PRIMARY KEY CLUSTERED ([GolferId] ASC, [TournamentId] ASC, [Year] ASC),
	CONSTRAINT [FK_TournamentParticipation_GolferId_Golfer_GolferId]
		FOREIGN KEY ([GolferId]) REFERENCES [dbo].[Golfer] ([GolferId]),
	CONSTRAINT [FK_TournamentParticipation_TournamentId_Tournament_TournamentId]
		FOREIGN KEY ([TournamentId]) REFERENCES [dbo].[Tournament] ([TournamentId])
);
GO

CREATE INDEX [IX_TournamentParticipation_GolferId] ON [dbo].[TournamentParticipation] ([GolferId]);
CREATE INDEX [IX_TournamentParticipation_TournamentId] ON [dbo].[TournamentParticipation] ([TournamentId]);
GO