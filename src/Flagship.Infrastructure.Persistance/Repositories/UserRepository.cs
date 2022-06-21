using Flagship.Core.Entities;
using Flagship.Core.Interfaces;
using Flagship.Infrastructure.Common;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Flagship.Infrastructure.Persistance.Repositories {
    public class UserRepository : IUserRepository {
        #region DataMembers and Properties
        private readonly IBaseRepository _baseRepository;
        #endregion

        #region Constructor
        public UserRepository(IBaseRepository baseRepository) {
            _baseRepository = baseRepository;
        }
        #endregion

        #region SQL Procedures
        private const string ProcUserGetAll = "usp_User_GetAll";
        private const string ProcUserGetAllActive = "usp_User_GetAllActive";
        private const string ProcUserGetUserById = "usp_User_GetByUser_Id";
        private const string ProcUserGetByLoginNameAndPassword = "usp_User_GetByLoginNameAndPassword";
        #endregion

        #region SQL Table Columns
        private const string USERID = "user_id";
        private const string LOGINNAME = "login_name";
        private const string DISPLAYNAME = "display_name";
        private const string FIRSTNAME = "first_name";
        private const string LASTNAME = "last_name";
        private const string LOGINPASSWORD = "login_password";
        private const string ISACTIVE = "is_active";
        private const string CREATEDBY = "created_by";
        private const string CREATEDDATE = "created_date";
        private const string CREATEDTIME = "created_time";
        private const string UPDATEDBY = "updated_by";
        private const string UPDATEDDATE = "updated_date";
        private const string UPDATEDTIME = "updated_time";
        private const string ROLENAME = "role_name";
        #endregion

        #region Functions
        public async Task<IList<User>> GetAll() {
            List<User> users = new();

            using (SqlConnection sqlConnection = _baseRepository.GetConnection()) {
                using (SqlCommand sqlCommand = _baseRepository.GetSqlCommand(sqlConnection, ProcUserGetAll, true)) {
                    using (var reader = await sqlCommand.ExecuteReaderAsync()) {
                        while (await reader.ReadAsync()) {
                            User user = new User() {
                                UserId = Conversion.ToInt(reader[USERID]),
                                LoginName = Conversion.ToString(reader[LOGINNAME]),
                                LoginPassword = Conversion.ToString(reader[LOGINPASSWORD]),
                                FirstName = Conversion.ToString(reader[FIRSTNAME]),
                                LastName = Conversion.ToString(reader[LASTNAME]),
                                DisplayName = Conversion.ToString(reader[DISPLAYNAME]),
                                IsActive = Conversion.ToBool(reader[ISACTIVE]),
                                CreatedBy = Conversion.ToInt(reader[CREATEDBY]),
                                CreatedDate = Conversion.ToDateTime(reader[CREATEDDATE]),
                                CreatedTime = Conversion.ToTimeSpan(reader[CREATEDTIME]),
                                UpdatedBy = reader[UPDATEDBY] != DBNull.Value ? Conversion.ToInt(reader[UPDATEDBY]) : null,
                                UpdatedDate = reader[UPDATEDDATE] != DBNull.Value ? Conversion.ToDateTime(reader[UPDATEDDATE]) : null,
                                UpdatedTime = reader[UPDATEDTIME] != DBNull.Value ? Conversion.ToTimeSpan(reader[UPDATEDTIME]) : null,
                            };

                            users.Add(user);
                        }
                    }
                }
            };

            return users;
        }
        public async Task<IList<User>> GetAllActive() {
            List<User> users = new();
            using (SqlConnection sqlConnection = _baseRepository.GetConnection()) {
                using SqlCommand sqlCommand = _baseRepository.GetSqlCommand(sqlConnection, ProcUserGetAllActive, true);
                using var reader = await sqlCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync()) {
                    User user = new User() {
                        UserId = Conversion.ToInt(reader[USERID]),
                        LoginName = Conversion.ToString(reader[LOGINNAME]),
                        LoginPassword = Conversion.ToString(reader[LOGINPASSWORD]),
                        FirstName = Conversion.ToString(reader[FIRSTNAME]),
                        LastName = Conversion.ToString(reader[LASTNAME]),
                        DisplayName = Conversion.ToString(reader[DISPLAYNAME]),
                        IsActive = Conversion.ToBool(reader[ISACTIVE]),
                        CreatedBy = Conversion.ToInt(reader[CREATEDBY]),
                        CreatedDate = Conversion.ToDateTime(reader[CREATEDDATE]),
                        CreatedTime = Conversion.ToTimeSpan(reader[CREATEDTIME]),
                        UpdatedBy = reader[UPDATEDBY] != DBNull.Value ? Conversion.ToInt(reader[UPDATEDBY]) : null,
                        UpdatedDate = reader[UPDATEDDATE] != DBNull.Value ? Conversion.ToDateTime(reader[UPDATEDDATE]) : null,
                        UpdatedTime = reader[UPDATEDTIME] != DBNull.Value ? Conversion.ToTimeSpan(reader[UPDATEDTIME]) : null,
                    };

                    users.Add(user);
                }
            };

            return users;
        }
        public async Task<User> GetAllById(Int64 userId) {
            User user = null;

            using (SqlConnection sqlConnection = _baseRepository.GetConnection()) {
                using SqlCommand sqlCommand = _baseRepository.GetSqlCommand(sqlConnection, ProcUserGetUserById, true);
                sqlCommand.Parameters.Add(_baseRepository.GetInParameter("@UserId", SqlDbType.Int, userId));

                using var reader = await sqlCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync()) {
                    user = new User() {
                        UserId = Conversion.ToInt(reader[USERID]),
                        LoginName = Conversion.ToString(reader[LOGINNAME]),
                        LoginPassword = Conversion.ToString(reader[LOGINPASSWORD]),
                        FirstName = Conversion.ToString(reader[FIRSTNAME]),
                        LastName = Conversion.ToString(reader[LASTNAME]),
                        DisplayName = Conversion.ToString(reader[DISPLAYNAME]),
                        IsActive = Conversion.ToBool(reader[ISACTIVE]),
                        CreatedBy = Conversion.ToInt(reader[CREATEDBY]),
                        CreatedDate = Conversion.ToDateTime(reader[CREATEDDATE]),
                        CreatedTime = Conversion.ToTimeSpan(reader[CREATEDTIME]),
                        UpdatedBy = reader[UPDATEDBY] != DBNull.Value ? Conversion.ToInt(reader[UPDATEDBY]) : null,
                        UpdatedDate = reader[UPDATEDDATE] != DBNull.Value ? Conversion.ToDateTime(reader[UPDATEDDATE]) : null,
                        UpdatedTime = reader[UPDATEDTIME] != DBNull.Value ? Conversion.ToTimeSpan(reader[UPDATEDTIME]) : null,
                    };
                }
            };

            return user;
        }
        public async Task<User> Login(string loginName, string loginPassword) {
            User user = null;
            using (SqlConnection sqlConnection = _baseRepository.GetConnection()) {
                using SqlCommand sqlCommand = _baseRepository.GetSqlCommand(sqlConnection, ProcUserGetByLoginNameAndPassword);
                sqlCommand.Parameters.Add(_baseRepository.GetInParameter("@LoginName", SqlDbType.NVarChar, loginName));
                sqlCommand.Parameters.Add(_baseRepository.GetInParameter("@LoginPassword", SqlDbType.NVarChar, loginPassword));
                
                using var reader = await sqlCommand.ExecuteReaderAsync();
                while (await reader.ReadAsync()) {
                    user = new User() {
                        UserId = Conversion.ToInt(reader[USERID]),
                        LoginName = Conversion.ToString(reader[LOGINNAME]),
                        FirstName = Conversion.ToString(reader[FIRSTNAME]),
                        LastName = Conversion.ToString(reader[LASTNAME]),
                        DisplayName = Conversion.ToString(reader[DISPLAYNAME]),
                        IsActive = Conversion.ToBool(reader[ISACTIVE]),
                        CreatedBy = Conversion.ToInt(reader[CREATEDBY]),
                        CreatedDate = Conversion.ToDateTime(reader[CREATEDDATE]),
                        CreatedTime = Conversion.ToTimeSpan(reader[CREATEDTIME]),
                        UpdatedBy = reader[UPDATEDBY] != DBNull.Value ? Conversion.ToInt(reader[UPDATEDBY]) : null,
                        UpdatedDate = reader[UPDATEDDATE] != DBNull.Value ? Conversion.ToDateTime(reader[UPDATEDDATE]) : null,
                        UpdatedTime = reader[UPDATEDTIME] != DBNull.Value ? Conversion.ToTimeSpan(reader[UPDATEDTIME]) : null,
                        RoleName = reader[ROLENAME] != DBNull.Value ? Conversion.ToString(reader[ROLENAME]) : String.Empty
                    };
                }
            };

            return user;
        }

        #endregion Functions
    }
}
