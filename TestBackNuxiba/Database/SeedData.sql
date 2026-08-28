USE CCenterRIA;
GO

SET XACT_ABORT ON;
GO

BEGIN TRANSACTION;
GO

/* ============================================================
   1. Clean previous test data
   ============================================================ */

DELETE FROM ccloglogin;
DELETE FROM ccUsers;
DELETE FROM ccRIACat_Areas;
GO

/* ============================================================
   2. Reset identities
   ============================================================ */

DBCC CHECKIDENT ('ccloglogin', RESEED, 0);
DBCC CHECKIDENT ('ccUsers', RESEED, 0);
DBCC CHECKIDENT ('ccRIACat_Areas', RESEED, 0);
GO

/* ============================================================
   3. Areas
   ============================================================ */

INSERT INTO ccRIACat_Areas
(
    AreaName,
    StatusArea,
    CreateDate
)
VALUES
(
    'Development',
    1,
    '2026-01-01 08:00:00'
),
(
    'Support',
    1,
    '2026-01-01 08:00:00'
),
(
    'Administration',
    1,
    '2026-01-01 08:00:00'
);
GO

/* ============================================================
   4. Users
   ============================================================ */

INSERT INTO ccUsers
(
    Login,
    Nombres,
    ApellidoPaterno,
    ApellidoMaterno,
    Password,
    TipoUser_id,
    Status,
    fCreate,
    IDArea,
    LastLoginAttempt
)
VALUES
(
    'test.user1',
    'Carlos',
    'Gomez',
    'Ramirez',
    'TestPassword',
    NULL,
    1,
    '2026-01-01 08:00:00',
    1,
    NULL
),
(
    'test.user2',
    'Ana',
    'Martinez',
    'Lopez',
    'TestPassword',
    NULL,
    1,
    '2026-01-01 08:00:00',
    2,
    NULL
),
(
    'test.user3',
    'Luis',
    'Hernandez',
    'Torres',
    'TestPassword',
    NULL,
    1,
    '2026-01-01 08:00:00',
    3,
    NULL
),
(
    'test.user4',
    'Maria',
    'Sanchez',
    'Flores',
    'TestPassword',
    NULL,
    1,
    '2026-01-01 08:00:00',
    1,
    NULL
);
GO

/* ============================================================
   5. Login / Logout records
   ============================================================ */

/*
    User 1
    ------------------------------------------------------------
    January:
        08:00 -> 17:00 = 9 hours
        08:30 -> 18:30 = 10 hours

    February:
        08:00 -> 16:00 = 8 hours

    March:
        09:00 -> 13:00 = 4 hours

    TOTAL = 31 hours
*/

INSERT INTO ccloglogin
(
    User_id,
    Extension,
    TipoMov,
    fecha
)
VALUES

-- User 1 - January
(1, 1001, 1, '2026-01-05 08:00:00'),
(1, 1001, 0, '2026-01-05 17:00:00'),

(1, 1001, 1, '2026-01-06 08:30:00'),
(1, 1001, 0, '2026-01-06 18:30:00'),

-- User 1 - February
(1, 1001, 1, '2026-02-10 08:00:00'),
(1, 1001, 0, '2026-02-10 16:00:00'),

-- User 1 - March
(1, 1001, 1, '2026-03-15 09:00:00'),
(1, 1001, 0, '2026-03-15 13:00:00');


/*
    User 2
    ------------------------------------------------------------
    January:
        09:00 -> 12:00 = 3 hours

    February:
        09:00 -> 13:00 = 4 hours

    TOTAL = 7 hours
*/

INSERT INTO ccloglogin
(
    User_id,
    Extension,
    TipoMov,
    fecha
)
VALUES

-- User 2 - January
(2, 1002, 1, '2026-01-05 09:00:00'),
(2, 1002, 0, '2026-01-05 12:00:00'),

-- User 2 - February
(2, 1002, 1, '2026-02-05 09:00:00'),
(2, 1002, 0, '2026-02-05 13:00:00');


/*
    User 3
    ------------------------------------------------------------
    January:
        08:00 -> 16:00 = 8 hours

    February:
        10:00 -> 17:00 = 7 hours

    TOTAL = 15 hours
*/

INSERT INTO ccloglogin
(
    User_id,
    Extension,
    TipoMov,
    fecha
)
VALUES

-- User 3 - January
(3, 1003, 1, '2026-01-10 08:00:00'),
(3, 1003, 0, '2026-01-10 16:00:00'),

-- User 3 - February
(3, 1003, 1, '2026-02-10 10:00:00'),
(3, 1003, 0, '2026-02-10 17:00:00');


/*
    User 4
    ------------------------------------------------------------
    January:
        08:00 -> 17:00 = 9 hours

    February:
        08:00 -> 17:30 = 9.5 hours

    March:
        08:30 -> 12:30 = 4 hours

    TOTAL = 22.5 hours
*/

INSERT INTO ccloglogin
(
    User_id,
    Extension,
    TipoMov,
    fecha
)
VALUES

-- User 4 - January
(4, 1004, 1, '2026-01-15 08:00:00'),
(4, 1004, 0, '2026-01-15 17:00:00'),

-- User 4 - February
(4, 1004, 1, '2026-02-15 08:00:00'),
(4, 1004, 0, '2026-02-15 17:30:00'),

-- User 4 - March
(4, 1004, 1, '2026-03-15 08:30:00'),
(4, 1004, 0, '2026-03-15 12:30:00');
GO

COMMIT TRANSACTION;
GO

/* ============================================================
   6. Verification
   ============================================================ */

SELECT
    u.User_id,
    u.Login,
    u.Nombres,
    u.ApellidoPaterno,
    u.ApellidoMaterno,
    a.AreaName
FROM ccUsers u
LEFT JOIN ccRIACat_Areas a
    ON a.IDArea = u.IDArea
ORDER BY u.User_id;
GO

SELECT
    l.LogLoginId,
    l.User_id,
    u.Login,
    l.Extension,
    CASE
        WHEN l.TipoMov = 1 THEN 'LOGIN'
        ELSE 'LOGOUT'
    END AS TipoMovimiento,
    l.fecha
FROM ccloglogin l
INNER JOIN ccUsers u
    ON u.User_id = l.User_id
ORDER BY
    l.User_id,
    l.fecha;
GO