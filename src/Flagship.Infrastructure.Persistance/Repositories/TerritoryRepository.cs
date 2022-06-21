using Flagship.Core.Entities;
using Flagship.Core.Interfaces;
using Flagship.Domain.Entities;
using Flagship.Infrastructure.Common;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Flagship.Infrastructure.Persistance.Repositories {
    public class TerritoryRepository : ITerritoryRepository {
        #region Properties and Data Members
        private readonly IBaseRepository _baseRepository;
        #endregion

        #region Constructor
        public TerritoryRepository(IBaseRepository baseRepository) {
            _baseRepository = baseRepository;
        }
        #endregion

        #region SQL Procedures
        private const string PROC_PROVINCE_GETALL = "[dbo].[usp_Province_GetAll]";
        private const string PROC_DIVISION_GETALL = "[dbo].[usp_Division_GetAll]";

        #endregion SQL Procedures

        #region SQL Table Columns
        private const string PROVINCEID = "province_id";
        private const string PROVINCENAME = "province_name";
        private const string DIVISIONID = "division_id";
        private const string DIVISIONNAME = "division_name";

        #endregion SQL Table Columns

        #region Mappers
        private static Province ProvinceMapper(IDataReader reader) {
            Province province = new() {
                ProvinceId = (reader[PROVINCEID] != DBNull.Value) ? Conversion.ToByte(reader[PROVINCEID]) : byte.MinValue,
                ProvinceName = (reader[PROVINCENAME] != DBNull.Value) ? Conversion.ToString(reader[PROVINCENAME]) : string.Empty,
            };
            return province;
        }
        private static Division DivisionMapper(IDataReader reader) {
            Division division = new() {
                DivisionId = (reader[DIVISIONID] != DBNull.Value) ? Conversion.ToByte(reader[DIVISIONID]) : byte.MinValue,
                ProvinceId = (reader[PROVINCEID] != DBNull.Value) ? Conversion.ToByte(reader[PROVINCEID]) : byte.MinValue,
                DivisionName = (reader[DIVISIONNAME] != DBNull.Value) ? Conversion.ToString(reader[DIVISIONNAME]) : string.Empty,
            };
            return division;
        }
        #endregion

        #region Functions
        public async Task<IList<Province>> GetAllProvince(bool isActive = true) {
            List<Province> proviceList = new();
            using SqlConnection sqlConnection = _baseRepository.GetConnection();
            using (SqlCommand sqlCommand = _baseRepository.GetSqlCommand(sqlConnection, PROC_PROVINCE_GETALL)) {
                sqlCommand.Parameters.Add(_baseRepository.GetInParameter("@IsActive", SqlDbType.Int, isActive));

                using (var reader = await sqlCommand.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        proviceList.Add(ProvinceMapper(reader));
                    }
                };
            }
            return proviceList;
        }
        public async Task<IList<Division>> GetAllDivisions(bool isActive = true) {
            List<Division> divisionList = new();

            using SqlConnection sqlConnection = _baseRepository.GetConnection();
            using (SqlCommand sqlCommand = _baseRepository.GetSqlCommand(sqlConnection, PROC_DIVISION_GETALL)) {
                sqlCommand.Parameters.Add(_baseRepository.GetInParameter("@IsActive", SqlDbType.Bit, isActive));
                using (var reader = await sqlCommand.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        divisionList.Add(DivisionMapper(reader));
                    }
                };

            }
            return divisionList;
        }
        #endregion Functions
    }
}
