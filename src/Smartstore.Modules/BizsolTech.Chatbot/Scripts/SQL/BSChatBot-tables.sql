USE [SS5Db]
GO

/****** Object:  Table [dbo].[BusinessPage]    Script Date: 27/11/2023 9:44:32 am ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[BusinessPage](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AdminId] [int] NOT NULL,
	[FBPageId] [bigint] NULL,
	[FBAccessToken] [nvarchar](max) NULL,
	[FBStatus] [bit] NOT NULL,
	[FBWebhookVerifyToken] [nvarchar](max) NULL,
	[FBWebhookStatus] [bit] NOT NULL,
	[OpenAPIKey] [nvarchar](max) NULL,
	[OpenAPIStatus] [bit] NOT NULL,
	[AzureOpenAPIKey] [nvarchar](max) NULL,
	[AzureOpenAPIStatus] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedOnUtc] [datetime] NOT NULL,
	[UpdatedOnUtc] [datetime] NOT NULL,
	[WelcomeMessage] [nvarchar](1000) NULL,
	[Description] [nvarchar](max) NULL,
	[Instruction] [nvarchar](max) NOT NULL
);



CREATE TABLE BusinessFact (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    BusinessPageId INT NOT NULL,
    Fact NVARCHAR(MAX),
    [Text] NVARCHAR(MAX),
    CreatedOnUtc DATETIME NOT NULL,
    UpdatedOnUtc DATETIME NOT NULL
);

CREATE TABLE BusinessChat (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    BusinessPageId INT NOT NULL,
    SenderId NVARCHAR(MAX) DEFAULT '',
    Question NVARCHAR(MAX) DEFAULT '',
    Answer NVARCHAR(MAX) DEFAULT '',
    CreatedOnUtc DATETIME NOT NULL,
    UpdatedOnUtc DATETIME NOT NULL
);

CREATE TABLE BusinessDocument(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    BusinessPageId INT NOT NULL,
    Name NVARCHAR(MAX) NOT NULL,
    FileUrl NVARCHAR(MAX) NOT NULL,
    CRC NVARCHAR(MAX) NOT NULL,
    Size INT NULL,
    Extension NVARCHAR(MAX) NOT NULL,
    OpenAIFileID NVARCHAR(MAX) NOT NULL,
    CreatedBy INT NOT NULL,
    ModifiedBy INT NULL,
    Deleted BIT NOT NULL,
    IsActive BIT NOT NULL,
    CreatedOnUtc DATETIME NOT NULL,
    UpdatedOnUtc DATETIME NOT NULL
);