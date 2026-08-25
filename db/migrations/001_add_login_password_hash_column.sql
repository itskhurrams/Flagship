-- Draft migration — review and adjust table name before running.
-- Adds a column to hold PBKDF2 password hashes alongside the existing plaintext
-- login_password column. Nullable for now so existing rows aren't broken until
-- they're backfilled by the one-time hashing pass (see README.md in this folder).

ALTER TABLE dbo.Users
ADD login_password_hash NVARCHAR(255) NULL;
