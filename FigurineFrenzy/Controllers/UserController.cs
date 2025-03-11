using DBAccess.Entites;
using FigurineFrenzeyViewModel.User;
using FigurineFrenzy.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.TokenService;
using Service.UserService;

namespace FigurineFrenzy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _user;
        private readonly ITokenService _token;
        
        public UserController(IUserService user, ITokenService token)
        {
            _user = user;
            _token = token;
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public async Task<IActionResult> GetUserProfile()
        {
            string header = Request.Headers["Authorization"].ToString();
            if (header != null && header.Length > 0)
            {
                string token = header.Split(" ")[1];
                if (token != null)
                {
                    var checkToken = await _token.CheckTokenAsync(token);
                    if (checkToken != null && checkToken.Role == "User")
                    {
                        User getUser = await _user.GetAsync(checkToken.AccountId);

                        UserInfoViewModel userInfo = new UserInfoViewModel
                        {
                            FullName = getUser.FullName,
                            Address = getUser.Address,
                            DateOfBirth = getUser.DateOfBirth.Value,
                            Email = getUser.Email,
                            Phone = checkToken.Phone,
                            
                        };
                        if (userInfo != null)
                        {   
                            return Ok(userInfo);
                        }
                        else return NotFound("Can't get User Info");
                    }
                    else return Unauthorized();
                }
                else return Unauthorized();
            }return Unauthorized();

        }

        [Authorize(Roles = "User")]
        [HttpPut("Update")]
        public async Task<IActionResult> Update(CreateUserViewModel userInfoView)
        {
            string header = Request.Headers["Authorization"].ToString();
            if (header != null && header.Length > 0)
            {
                string token = header.Split(" ")[1];
                if (token != null)
                {
                    var checkToken = await _token.CheckTokenAsync(token);
                    if (checkToken != null && checkToken.Role == "User")
                    {
                        var getUserInfo = await _user.GetAsync(checkToken.AccountId);
                        if (getUserInfo != null)
                        {
                            var updateUserInfo = await _user.UpdateAsync(userInfoView, checkToken.AccountId);
                            if (updateUserInfo == Service.Enum.RESPONSECODE.OK)
                            {
                                return Ok();
                            }
                            else return StatusCode(500);
                        }
                        return StatusCode(500);
                    }
                    else return Unauthorized();
                }
                else return Unauthorized();
            }
            else return Unauthorized();
        }
    }
}
