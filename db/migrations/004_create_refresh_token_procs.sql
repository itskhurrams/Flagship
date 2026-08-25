-- Draft migration — review and adjust table/column names before running.
-- Stored procedures backing Flagship.Infrastructure.Persistance.Repositories.RefreshTokenRepository.

CREATE OR ALTER PROCEDURE dbo.usp_RefreshToken_Insert
    @UserId BIGINT,
    @TokenHash NVARCHAR(64),
    @CreatedAtUtc DATETIME2,
    @ExpiresAtUtc DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.RefreshTokens (user_id, token_hash, created_at_utc, expires_at_utc)
    VALUES (@UserId, @TokenHash, @CreatedAtUtc, @ExpiresAtUtc);
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_RefreshToken_GetByTokenHash
    @TokenHash NVARCHAR(64)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (1)
        refresh_token_id,
        user_id,
        token_hash,
        created_at_utc,
        expires_at_utc,
        revoked_at_utc,
        replaced_by_token_hash
    FROM dbo.RefreshTokens
    WHERE token_hash = @TokenHash;
END
GO

-- Marks a token used/rotated. Only touches rows that aren't already revoked, so
-- a concurrent double-use of the same token can't both "win" and each rotate to
-- a different replacement — the service layer's reuse check then treats the
-- loser's attempt as a replay and revokes the whole session family.
CREATE OR ALTER PROCEDURE dbo.usp_RefreshToken_Revoke
    @TokenHash NVARCHAR(64),
    @ReplacedByTokenHash NVARCHAR(64) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.RefreshTokens
    SET revoked_at_utc = SYSUTCDATETIME(),
        replaced_by_token_hash = @ReplacedByTokenHash
    WHERE token_hash = @TokenHash
      AND revoked_at_utc IS NULL;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_RefreshToken_RevokeAllForUser
    @UserId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.RefreshTokens
    SET revoked_at_utc = SYSUTCDATETIME()
    WHERE user_id = @UserId
      AND revoked_at_utc IS NULL;
END
GO
