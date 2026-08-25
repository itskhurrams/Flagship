# Password-hashing migration (draft — not applied)

These scripts back the app-code change that made `UserRepository.Login` verify a
PBKDF2 password hash in C# instead of having SQL compare a plaintext password
(`usp_User_GetByLoginNameAndPassword`). None of this has been run against any
database — review and adjust table/column names to match the real schema, then
apply in order.

## Rollout order

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
