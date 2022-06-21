using Flagship.Application.Interfaces;
using Flagship.Core.Interfaces;
using Flagship.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace Flagship.Application.Services {
    public class TerritoryService : ITerritoryService
    {
        #region Properties and Data Members
        private readonly ITerritoryRepository _territoryRepository;
        private readonly IMemoryCacheProvider _memoryCacheProvider;
        //private readonly IDataPermissionRepository _dataPermissionRepository;
        private readonly IConfiguration _configuration;
        private static readonly SemaphoreSlim GetUsersSemaphore = new SemaphoreSlim(1, 1);
        #endregion

        #region Constructor
        public TerritoryService(ITerritoryRepository territoryRepository, IMemoryCacheProvider memoryCacheProvider, IConfiguration configuration)
        {
            _territoryRepository = territoryRepository;
            _memoryCacheProvider = memoryCacheProvider;
            //_dataPermissionRepository = dataPermissionRepository;
            _configuration = configuration;
        }
        #endregion

        public async Task<IList<Division>> GetAllDivisions(bool isActive = true)
        {
            return await _territoryRepository.GetAllDivisions(isActive);
        }
    }
}
