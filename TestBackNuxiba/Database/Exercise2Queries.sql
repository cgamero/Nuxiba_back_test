-- ============================================================
-- Exercise 2.1
-- User with the MOST logged-in time
-- ============================================================
WITH Movements AS
(
    SELECT
        LogLoginId,
        User_id,
        TipoMov,
        fecha,

        LEAD(TipoMov) OVER
        (
            PARTITION BY User_id
            ORDER BY fecha, LogLoginId
        ) AS NextTipoMov,

        LEAD(fecha) OVER
        (
            PARTITION BY User_id
            ORDER BY fecha, LogLoginId
        ) AS NextFecha

    FROM ccloglogin
),
Sessions AS
(
    SELECT
        User_id,
        fecha AS LoginDate,
        NextFecha AS LogoutDate,
        DATEDIFF_BIG(SECOND, fecha, NextFecha) AS DurationSeconds
    FROM Movements
    WHERE TipoMov = 1
      AND NextTipoMov = 0
),
Totals AS
(
    SELECT
        User_id,
        SUM(DurationSeconds) AS TotalSeconds
    FROM Sessions
    GROUP BY User_id
)
SELECT TOP 1
    t.User_id,
    u.Login,
    u.Nombres,
    u.ApellidoPaterno,
    u.ApellidoMaterno,
    t.TotalSeconds,
    CONCAT(
        t.TotalSeconds / 86400,
        ' days, ',
        (t.TotalSeconds % 86400) / 3600,
        ' hours, ',
        (t.TotalSeconds % 3600) / 60,
        ' minutes, ',
        t.TotalSeconds % 60,
        ' seconds'
    ) AS TotalTime
FROM Totals t
INNER JOIN ccUsers u
    ON u.User_id = t.User_id
ORDER BY t.TotalSeconds DESC;

-- ============================================================
-- Exercise 2.2
-- User with the LEAST logged-in time
-- ============================================================
WITH Movements AS
(
    SELECT
        LogLoginId,
        User_id,
        TipoMov,
        fecha,

        LEAD(TipoMov) OVER
        (
            PARTITION BY User_id
            ORDER BY fecha, LogLoginId
        ) AS NextTipoMov,

        LEAD(fecha) OVER
        (
            PARTITION BY User_id
            ORDER BY fecha, LogLoginId
        ) AS NextFecha

    FROM ccloglogin
),
Sessions AS
(
    SELECT
        User_id,
        fecha AS LoginDate,
        NextFecha AS LogoutDate,
        DATEDIFF_BIG(SECOND, fecha, NextFecha) AS DurationSeconds
    FROM Movements
    WHERE TipoMov = 1
      AND NextTipoMov = 0
),
Totals AS
(
    SELECT
        User_id,
        SUM(DurationSeconds) AS TotalSeconds
    FROM Sessions
    GROUP BY User_id
)
SELECT TOP 1
    t.User_id,
    u.Login,
    u.Nombres,
    u.ApellidoPaterno,
    u.ApellidoMaterno,
    t.TotalSeconds,
    CONCAT(
        t.TotalSeconds / 86400,
        ' days, ',
        (t.TotalSeconds % 86400) / 3600,
        ' hours, ',
        (t.TotalSeconds % 3600) / 60,
        ' minutes, ',
        t.TotalSeconds % 60,
        ' seconds'
    ) AS TotalTime
FROM Totals t
INNER JOIN ccUsers u
    ON u.User_id = t.User_id
ORDER BY t.TotalSeconds ASC;

-- ============================================================
-- Exercise 2.3
-- Average logged-in time by user/month
-- ============================================================
WITH Movements AS
(
    SELECT
        LogLoginId,
        User_id,
        TipoMov,
        fecha,

        LEAD(TipoMov) OVER
        (
            PARTITION BY User_id
            ORDER BY fecha, LogLoginId
        ) AS NextTipoMov,

        LEAD(fecha) OVER
        (
            PARTITION BY User_id
            ORDER BY fecha, LogLoginId
        ) AS NextFecha

    FROM ccloglogin
),
Sessions AS
(
    SELECT
        User_id,
        fecha AS LoginDate,
        NextFecha AS LogoutDate,

        DATEDIFF_BIG(
            SECOND,
            fecha,
            NextFecha
        ) AS DurationSeconds

    FROM Movements

    WHERE TipoMov = 1
      AND NextTipoMov = 0
),
MonthlyAverage AS
(
    SELECT
        User_id,
        YEAR(LoginDate) AS LoginYear,
        MONTH(LoginDate) AS LoginMonth,

        AVG(CAST(DurationSeconds AS DECIMAL(18,2)))
            AS AverageSeconds

    FROM Sessions

    GROUP BY
        User_id,
        YEAR(LoginDate),
        MONTH(LoginDate)
)
SELECT
    ma.User_id,
    u.Login,
    ma.LoginYear,
    ma.LoginMonth,

    ma.AverageSeconds,

    CONCAT(
        CAST(ma.AverageSeconds / 86400 AS BIGINT),
        ' days, ',

        CAST(
            (ma.AverageSeconds % 86400) / 3600
            AS BIGINT
        ),
        ' hours, ',

        CAST(
            (ma.AverageSeconds % 3600) / 60
            AS BIGINT
        ),
        ' minutes, ',

        CAST(
            ma.AverageSeconds % 60
            AS BIGINT
        ),
        ' seconds'
    ) AS AverageTime

FROM MonthlyAverage ma

INNER JOIN ccUsers u
    ON u.User_id = ma.User_id

ORDER BY
    ma.User_id,
    ma.LoginYear,
    ma.LoginMonth;