# Draft migrations — not applied

Two independent app-code changes need matching schema changes. Neither has been
run against any database — review and adjust table/column names to match the
real schema before applying.

- `001`–`002`: password hashing (`UserRepository.Login`).
- `003`–`004`: refresh-token rotation (`RefreshTokenService`, `RefreshTokenRepository`).

## Password hashing rollout order

1. **`001_add_login_password_hash_column.sql`** — adds the nullable
   `login_password_hash` column.
2. **Backfill existing rows** — for every user, compute
   `Flagship.Infrastructure.Common.PasswordHasher.Hash(plaintextPassword)` and
   write it to `login_password_hash`. This has to happen in .NET (or any PBKDF2
   implementation compatible with the format below), not T-SQL. One-off example:

   ```csharp
   // Run once, then discard. Needs a reference to Flagship.Infrastructure.Common
   // and Microsoft.Data.SqlClient.
   using var connection = new SqlConnection(connectionString);
   connection.Open();

   using var selectCommand = new SqlCommand(
       "SELECT user_id, login_password FROM dbo.Users WHERE login_password_hash IS NULL", connection);
   var rows = new List<(long UserId, string PlaintextPassword)>();
   using (var reader = selectCommand.ExecuteReader()) {
       while (reader.Read())
           rows.Add((reader.GetInt64(0), reader.GetString(1)));
   }

   foreach (var (userId, plaintextPassword) in rows) {
       var hash = PasswordHasher.Hash(plaintextPassword);
       using var updateCommand = new SqlCommand(
           "UPDATE dbo.Users SET login_password_hash = @Hash WHERE user_id = @UserId", connection);
       updateCommand.Parameters.AddWithValue("@Hash", hash);
       updateCommand.Parameters.AddWithValue("@UserId", userId);
       updateCommand.ExecuteNonQuery();
   }
   ```

3. **`002_create_usp_User_GetByLoginName.sql`** — creates the new lookup-by-name
   procedure the app now calls.
4. Deploy the updated application code (already done in this branch).
5. Once verified in production, drop `usp_User_GetByLoginNameAndPassword` and the
   plaintext `login_password` column — check first whether any other system
   still reads `login_password` before dropping it.

## Hash format

`PasswordHasher.Hash` (in `Flagship.Infrastructure.Common`) produces
`"{iterations}.{base64 salt}.{base64 key}"` using PBKDF2-HMAC-SHA256, a 16-byte
salt, a 32-byte derived key, and 210,000 iterations (OWASP's current minimum
recommendation for PBKDF2-SHA256). `PasswordHasher.Verify` reads the iteration
count back out of the stored hash, so raising `Iterations` later doesn't
invalidate hashes already written.

## Refresh-token rollout order

1. **`003_create_refresh_tokens_table.sql`** — creates `dbo.RefreshTokens`
   (adjust the `FK_RefreshTokens_Users` reference if your `Users` table or PK
   column is named differently).
2. **`004_create_refresh_token_procs.sql`** — creates the four supporting
   procedures (`Insert`, `GetByTokenHash`, `Revoke`, `RevokeAllForUser`).
3. Deploy the updated application code (already done in this branch) —
   `POST /api/v1/Account/Authenticate` now also returns a `RefreshToken` in the
   response's `AdditionalData`, and `POST /api/v1/Account/RefreshToken` exchanges
   a still-valid refresh token for a new access token + a new refresh token.

### Design notes

- Refresh tokens are opaque 256-bit random values (base64url), never JWTs —
  only their SHA-256 hash is stored, so a database read can't be replayed as a
  usable token. This replaces the old unused `TokenAuthentication:JWTRefreshTokenKey`
  config value, which has been removed.
- **Rotation with reuse detection**: each successful refresh revokes the token
  just used and issues a new one (`replaced_by_token_hash` records the chain).
  If an already-revoked token is presented again — a strong signal of a stolen
  or replayed token — `RefreshTokenService.Rotate` revokes *every* refresh token
  for that user, forcing re-authentication everywhere.
- `POST /api/v1/Account/Logout` now also revokes all of a user's refresh tokens,
  in addition to the existing `LoginLog` session-token bookkeeping (a separate,
  pre-existing audit mechanism this doesn't replace).
