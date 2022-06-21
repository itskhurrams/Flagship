using Flagship.Application.Interfaces;
using Flagship.Core.ViewModels;
using Flagship.Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flagship.API.Controllers {
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TerritoryController : BaseController {
        #region Data Members and Properties
        private readonly ITerritoryService _territoryService;
        #endregion

        #region Constructor
        public TerritoryController(ITerritoryService territoryService) {
            _territoryService = territoryService;
        }
        #endregion

        [HttpGet]
        [Route("GetDivision")]
        public async Task<ActionResult<Division>> GetDivision() {
            return Ok(await _territoryService.GetAllDivisions());
        }
    }
}
