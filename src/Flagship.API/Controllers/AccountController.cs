using Flagship.Application.Interfaces;
using Flagship.Core.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flagship.API.Controllers {
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AccountController : BaseController {
        #region Properties and Data Members
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        //private readonly IDataPermissionService _dataPermissionService;
        private readonly ILoginLogService _loginLogService;
        private readonly IRefreshTokenService _refreshTokenService;
        #endregion

        #region Constructor
        public AccountController(IUserService userService, IConfiguration configuration, ILoginLogService loginLogService, IRefreshTokenService refreshTokenService) {
            _userService = userService;
            _configuration = configuration;
            _loginLogService = loginLogService;
            _refreshTokenService = refreshTokenService;
        }
        #endregion
        [HttpPost]
        [AllowAnonymous]
        [Route("Authenticate")]
        public async Task<ActionResult<ResponseViewModel>> Login([FromBody] LoginViewModel login) {
            var response = new ResponseViewModel() { OperationStatus = false };

            if (ModelState.IsValid) {
                var user = await _userService.Login(login.LoginName, login.Password);
                if (user != null) {
                    var jwtToken = await _userService.GenerateToken(user.UserId, user.LoginName);
                    jwtToken.RefreshToken = await _refreshTokenService.Issue(user.UserId);
                    response.Data = user;
                    response.AdditionalData = jwtToken;
                    response.Message = "Login Successfully";
                    response.OperationStatus = true;
                    return Ok(response);
                }
                else {
                    response.Message = "Incorrect Username/Password";
                    return BadRequest(response);
                }
            }
            else {
                response.Message = "Invalid Request";
                return BadRequest(response);
            }
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("RefreshToken")]
        public async Task<ActionResult<ResponseViewModel>> RefreshToken([FromBody] RefreshTokenRequestViewModel request) {
            var response = new ResponseViewModel() { OperationStatus = false };

            if (!ModelState.IsValid) {
                response.Message = "Invalid Request";
                return BadRequest(response);
            }

            var rotation = await _refreshTokenService.Rotate(request.RefreshToken);
            if (rotation == null) {
                response.Message = "Invalid or expired refresh token";
                return Unauthorized(response);
            }

            var (userId, newRefreshToken) = rotation.Value;
            var user = await _userService.GetById(userId);
            if (user == null || !user.IsActive) {
                response.Message = "Invalid or expired refresh token";
                return Unauthorized(response);
            }

            var jwtToken = await _userService.GenerateToken(user.UserId, user.LoginName);
            jwtToken.RefreshToken = newRefreshToken;

            response.Data = user;
            response.AdditionalData = jwtToken;
            response.Message = "Token refreshed";
            response.OperationStatus = true;
            return Ok(response);
        }
        [HttpPost]
        [Route("Logout")]
        public async Task<ActionResult<ResponseViewModel>> Logout(Int64 userId, string sessionToken) {
            var responseVM = new ResponseViewModel() { OperationStatus = false };

            if (userId > 0 && !string.IsNullOrEmpty(sessionToken)) {
                responseVM.OperationStatus = await _loginLogService.UpdateBySessionTokenAndUserId(userId, sessionToken);
                await _refreshTokenService.RevokeAllForUser(userId);
                responseVM.Message = "Information has been saved";
                return Ok(responseVM);
            }
            else {
                responseVM.Message = "Invalid Request";
                return BadRequest(responseVM);
            }
        }
    }
}
