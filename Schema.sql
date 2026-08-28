CREATE TABLE ccRIACat_Areas
(
    IDArea INT IDENTITY(1,1) NOT NULL,
    AreaName NVARCHAR(100) NOT NULL,
    StatusArea BIT NOT NULL CONSTRAINT DF_ccRIACat_Areas_StatusArea DEFAULT 1,
    CreateDate DATETIME2 NOT NULL CONSTRAINT DF_ccRIACat_Areas_CreateDate DEFAULT GETDATE(),

    CONSTRAINT PK_ccRIACat_Areas PRIMARY KEY (IDArea)
);
GO

CREATE TABLE ccUsers
(
    User_id INT IDENTITY(1,1) NOT NULL,
    Login NVARCHAR(100) NOT NULL,
    Nombres NVARCHAR(100) NOT NULL,
    ApellidoPaterno NVARCHAR(100) NULL,
    ApellidoMaterno NVARCHAR(100) NULL,
    Password NVARCHAR(255) NULL,
    TipoUser_id INT NULL,
    Status INT NOT NULL CONSTRAINT DF_ccUsers_Status DEFAULT 1,
    fCreate DATETIME2 NOT NULL CONSTRAINT DF_ccUsers_fCreate DEFAULT GETDATE(),
    IDArea INT NULL,
    LastLoginAttempt DATETIME2 NULL,

    CONSTRAINT PK_ccUsers PRIMARY KEY (User_id),

    CONSTRAINT FK_ccUsers_Area
        FOREIGN KEY (IDArea)
        REFERENCES ccRIACat_Areas(IDArea)
);
GO

ALTER TABLE ccUsers
ADD CONSTRAINT UQ_ccUsers_Login UNIQUE (Login);
GO

CREATE TABLE ccloglogin
(
    LogLoginId BIGINT IDENTITY(1,1) NOT NULL,
    User_id INT NOT NULL,
    Extension INT NOT NULL,
    TipoMov INT NOT NULL,
    fecha DATETIME2 NOT NULL,

    CONSTRAINT PK_ccloglogin PRIMARY KEY (LogLoginId),

    CONSTRAINT FK_ccloglogin_User
        FOREIGN KEY (User_id)
        REFERENCES ccUsers(User_id),

    CONSTRAINT CK_ccloglogin_TipoMov
        CHECK (TipoMov IN (0, 1))
);
GO

CREATE INDEX IX_ccloglogin_User_Fecha
ON ccloglogin (User_id, fecha);
GO

CREATE INDEX IX_ccloglogin_User_TipoMov_Fecha
ON ccloglogin (User_id, TipoMov, fecha);
GO

SELECT TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;