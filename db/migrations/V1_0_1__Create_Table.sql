CREATE TABLE [dbo].[MemberType]
(
	MemberTypeId INT IDENTITY (1, 1) NOT NULL,
	[Name]       NVARCHAR(256)       NOT NULL,
	[Fee]        DECIMAL(19, 2)      NULL,
	CONSTRAINT [PK_MemberType_MemberTypeID]
		PRIMARY KEY CLUSTERED (MemberTypeId ASC),
	CONSTRAINT [AK_MemberType_Name]
		UNIQUE (Name)
);
GO

CREATE TABLE [dbo].[Member]
(
	[MemberID]     INT IDENTITY (1, 1) NOT NULL,
	[LastName]     NVARCHAR(256)       NOT NULL,
	[FirstName]    NVARCHAR(256)       NOT NULL,
	[MemberTypeID] INT                 NOT NULL,
	[Phone]        NVARCHAR(256)       NULL,
	[Handicap]     TINYINT             NULL,
	[JoinDate]     DATE                NOT NULL,
	[Coach]        INT                 NULL,
	[Gender]       NCHAR(1)            NOT NULL,
	CONSTRAINT [PK_Member_MemberID]
		PRIMARY KEY CLUSTERED ([MemberID] ASC),
	CONSTRAINT [FK_MemberType_Member_MemberTypeID]
		FOREIGN KEY ([MemberTypeID]) REFERENCES [dbo].[MemberType] (MemberTypeId)
);
GO

CREATE TABLE [dbo].[TournamentType]
(
	[TournamentTypeID] INT IDENTITY (1, 1) NOT NULL,
	[Name]             NVARCHAR(256)       NOT NULL,
	CONSTRAINT [PK_TournamentType_TournamentTypeID]
		PRIMARY KEY CLUSTERED ([TournamentTypeID] ASC)
);
GO

CREATE TABLE [dbo].[Tournament]
(
	[TournamentID]     INT IDENTITY (1, 1) NOT NULL,
	[Name]             NVARCHAR(256)       NOT NULL,
	[TournamentTypeID] INT                 NOT NULL,
	CONSTRAINT [PK_Tournament_TournamentID]
		PRIMARY KEY CLUSTERED ([TournamentID] ASC),
	CONSTRAINT [FK_TournamentType_Tournament_TournamentTypeID]
		FOREIGN KEY ([TournamentTypeID]) REFERENCES [dbo].[TournamentType] ([TournamentTypeID])
);
GO

CREATE TABLE [dbo].[TournamentParticipation]
(
	[MemberID]     INT      NOT NULL,
	[TournamentID] INT      NOT NULL,
	[Year]         NCHAR(4) NOT NULL,
	CONSTRAINT [PK_TournamentParticipation_MemberID_TournamentID_Year]
		PRIMARY KEY CLUSTERED ([MemberID] ASC, [TournamentID] ASC, [Year] ASC),
	CONSTRAINT [FK_Member_TournamentParticipation_MemberID]
		FOREIGN KEY ([MemberID]) REFERENCES [dbo].[Member] ([MemberID]),
	CONSTRAINT [FK_Tournament_TournamentParticipation_TournamentID]
		FOREIGN KEY ([TournamentID]) REFERENCES [dbo].[Tournament] ([TournamentID])
);
GO

ALTER TABLE [dbo].[Member]
	WITH CHECK ADD CONSTRAINT [FK_Coach] FOREIGN KEY ([Coach])
		REFERENCES [dbo].[Member] ([MemberID]);
GO
ALTER TABLE [dbo].[Member]
	CHECK CONSTRAINT [FK_Coach];
GO
