IF NOT EXISTS (SELECT * FROM sys.tables t WHERE t.name = 'Users')
    CREATE TABLE Users (
    [Id] [uniqueidentifier] NOT NULL,
    [Name] NVARCHAR(200) NOT NULL,
    [PhoneNumber] NVARCHAR(200),
    [PhoneNumber_CountryCode] NVARCHAR(200)
    );
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'Users') AND name = N'PK_Id')
    ALTER TABLE Users
    ADD CONSTRAINT PK_Id PRIMARY KEY CLUSTERED (Id);