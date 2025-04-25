using DBAccess.Entites;
using FigurineFrenzeyViewModel.Account;
using FigurineFrenzeyViewModel.Admin;
using FigurineFrenzeyViewModel.Role;
using FigurineFrenzy.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.AccountService;
using Service.AdminService;
using Service.HashService;
using Service.RoleService;
using Service.TokenService;
using Service.UserService;

namespace FigurineFrenzy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAccountService _account;
        private readonly IRoleService _role;
        private readonly IUserService _user;
        private readonly IHashService _hash;
        private readonly IValidator _validate;
        private readonly ITokenService _token;
        private readonly IAdminService _admin;
        private readonly string privateKey = "8243--jjseijuq[ojnj*@*&%))#(_^**...a.k';][]";

        public AdminController(IAccountService account, IRoleService role, IUserService user, IHashService hash, IValidator validate, ITokenService token, IAdminService admin)
        {

            _account = account;
            _role = role;
            _hash = hash;
            _validate = validate;
            _token = token;
            _admin = admin;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("CheckToken")]
        public async Task<IActionResult> CheckToken()
        {
            string header = Request.Headers["Authorization"].ToString();
            if (header != null && header.Length > 0)
            {
                string token = header.Split(" ")[1];
                if (token != null)
                {
                    var checkToken = await _token.CheckTokenAsync(token);
                    if (checkToken != null && checkToken.Role == "Admin")
                    {
                        return Ok();
                    }
                    else return Unauthorized();
                }
                return Unauthorized();
            }
            return Unauthorized();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Profile")]
        public async Task<IActionResult> GetAdminProfile()
        {
            string header = Request.Headers["Authorization"].ToString();
            if (header != null && header.Length > 0)
            {
                string token = header.Split(" ")[1];
                if (token != null)
                {
                    var checkToken = await _token.CheckTokenAsync(token);
                    if (checkToken != null && checkToken.Role == "Admin")
                    {
                        Admin adminInfo = await _admin.GetAsync(checkToken.AccountId);

                        AdminInfoViewModel adminInfoView = new AdminInfoViewModel
                        {
                            Email = adminInfo.Email,
                            Phone = checkToken.Phone
                        };
                        if(adminInfo != null)
                        {
                            return Ok(adminInfoView);

                        }
                        else return NotFound("Admin Info may be not exist");
                    }
                    else return Unauthorized();
                }
                else return Unauthorized();
            }
            else return Unauthorized();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            string header = Request.Headers["Authorization"].ToString();
            if (header != null && header.Length > 0)
            {
                string token = header.Split(" ")[1];
                if (token != null)
                {
                    var checkToken = await _token.CheckTokenAsync(token);
                    if (checkToken != null && checkToken.Role == "Admin")
                    {
                        var logout = await _token.DeleteTokenAsync(token, checkToken.AccountId);
                        if (logout)
                        {
                            return Ok();
                        }
                        else return StatusCode(500);
                    }
                    else return StatusCode(500);
                }
                return StatusCode(500);
            }
            return BadRequest("Can't get token from header");
        }


        [Authorize(Roles = "Admin")]
        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreateFullAccountAdminViewModel createFullAccount)
        {
            string header = Request.Headers["Authorization"].ToString();
            if (header != null && header.Length > 0)
            {
                string token = header.Split(" ")[1];
                if (token != null)
                {
                    var checkToken = await _token.CheckTokenAsync(token);
                    if (checkToken != null && checkToken.Role == "Admin")
                    {
                        if (ModelState.IsValid)
                        {
                            string accId = Guid.NewGuid().ToString();
                            RoleViewModel roleInfo = await _role.GetRoleIdByNameAsync("Admin");
                            createFullAccount.account.Password = _hash.SHA256(createFullAccount.account.Password + privateKey);
                            if (_validate.PhoneValidate(createFullAccount.account.Phone) && _validate.EmailValidate(createFullAccount.admin.Email))
                            {
                                //create account
                                var accResult = await _account.CreateAsync(createFullAccount.account, accId, roleInfo.Id);
                                if (accResult == Service.Enum.RESPONSECODE.OK)
                                {
                                    //create userInfo
                                    var adminResult = await _admin.CreateAsync(createFullAccount.admin, accId);
                                    if (adminResult == Service.Enum.RESPONSECODE.OK)
                                    {
                                        return Ok("Rigister Success");
                                    }
                                    else
                                    {
                                        return StatusCode(500);
                                    }
                                }
                                else if (accResult == Service.Enum.RESPONSECODE.INTERNALERROR)
                                {
                                    return StatusCode(500);
                                }
                                else
                                {
                                    return BadRequest("Account have already exist");
                                }
                            }
                            else return BadRequest("Your phone or email are incorect");
                        }
                        else
                        {
                            string message = string.Join(" | ", ModelState.Values
                                                .SelectMany(v => v.Errors)
                                                .Select(e => e.ErrorMessage));
                            return BadRequest(message);
                        }
                    }
                    return Unauthorized();
                }
                return Unauthorized();
            }
            return Unauthorized();
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginInfoViewModel loginInfo)
        {
            if (ModelState.IsValid)
            {
                //hashpassword
                loginInfo.Password = _hash.SHA256(loginInfo.Password + privateKey);
                //get role id
                RoleViewModel roleInfo = await _role.GetRoleIdByNameAsync("Admin");
                //check login
                Account loginResult = await _account.CheckLogin(loginInfo, roleInfo.Id);

                if (loginResult != null)
                {
                    //generate token
                    string token = _token.GenerateToken(loginResult.AccountId, loginResult.Phone, roleInfo.Name);
                    if (token != null)
                    {
                        //save token
                        bool saveResult = await _token.SaveToken(token, loginResult.AccountId);
                        if (saveResult)
                        {
                            return Ok(token);
                        }
                        else return StatusCode(500);
                    }
                    else return StatusCode(500);

                }
                else return BadRequest("Your login information inccrectr");
            }
            else
            {

            }
            {
                string message = string.Join(" | ", ModelState.Values
                      .SelectMany(v => v.Errors)
                      .Select(e => e.ErrorMessage));

                return BadRequest(message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("Update")]
        public async Task<IActionResult> Update(CreateAdminViewModel create)
        {
            string header = Request.Headers["Authorization"].ToString();
            if (header != null && header.Length > 0)
            {
                string token = header.Split(" ")[1];
                if (token != null)
                {
                    var checkToken = await _token.CheckTokenAsync(token);
                    if (checkToken != null && checkToken.Role == "Admin")
                    {
                        if (ModelState.IsValid)
                        {
                            var accResult = await _admin.UpdateAsync(create, checkToken.AccountId);
                            if (accResult == Service.Enum.RESPONSECODE.OK)
                            {
                                return Ok();
                            }
                            else return StatusCode(500);
                        }
                        else return StatusCode(400);
                    }
                    else return Unauthorized();
                }
                else return Unauthorized();
            }
            else return Unauthorized();
        }
    }
}
