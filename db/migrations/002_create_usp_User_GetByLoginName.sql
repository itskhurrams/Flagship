-- Draft migration — review and adjust table/column names before running.
-- Replaces usp_User_GetByLoginNameAndPassword (which compared the plaintext
-- password in SQL) with a lookup by login name only. The application now
-- verifies the PBKDF2 hash in code (Flagship.Infrastructure.Common.PasswordHasher)
-- instead of the database comparing plaintext.
--
-- usp_User_GetByLoginNameAndPassword can be dropped once this is deployed and
-- verified; it's left in place here for a safe rollback window.

CREATE OR ALTER PROCEDURE dbo.usp_User_GetByLoginName
    @LoginName NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        user_id,
        login_name,
        login_password_hash,
        display_name,
        first_name,
        last_name,
        is_active,
        created_by,
        created_date,
        created_time,
        updated_by,
        updated_date,
        updated_time,
        role_name
    FROM dbo.Users
    WHERE login_name = @LoginName
      AND is_active = 1;
END
