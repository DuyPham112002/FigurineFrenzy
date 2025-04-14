﻿using DBAccess.Entites;
using FigurineFrenzeyViewModel.Account;
using FigurineFrenzeyViewModel.Role;
using FigurineFrenzy.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Identity.Client;
using Service.AccountService;
using Service.AdminService;
using Service.EmailService;
using Service.HashService;
using Service.RoleService;
using Service.TokenService;
using Service.UserService;
using System.Security.Claims;

namespace FigurineFrenzy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _account;
        private readonly IRoleService _role;
        private readonly IUserService _user;
        private readonly IHashService _hash;
        private readonly IValidator _validate;
        private readonly ITokenService _token;
        private readonly IAdminService _admin;
        private readonly IEmailService _email;
        private readonly string privateKey = "8243--jjseijuq[ojnj*@*&%))#(_^**...a.k';][]";
        public AccountController(IAccountService account, IRoleService role, IUserService user, IHashService hash, IValidator validate, ITokenService token
            , IAdminService admin, IEmailService email)
        {
            _account = account;
            _role = role;
            _user = user;
            _hash = hash;
            _validate = validate;
            _token = token;
            _admin = admin;
            _email = email;
        }

        [Authorize(Roles = "User")]
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
                    if (checkToken != null && checkToken.Role == "User")
                    {
                        return Ok();
                    }
                    else return Unauthorized();
                }return Unauthorized();
            }return Unauthorized();
        }

        [Authorize(Roles = "User")]
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
                    if (checkToken != null && checkToken.Role == "User")
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

        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreateFullAccountViewModel createFullAccount)
        {
            if (ModelState.IsValid)
            {
                string accId = Guid.NewGuid().ToString();
                RoleViewModel roleInfo = await _role.GetRoleIdByNameAsync("User");
                createFullAccount.account.Password = _hash.SHA256(createFullAccount.account.Password + privateKey);
                if (_validate.PhoneValidate(createFullAccount.account.Phone) && _validate.EmailValidate(createFullAccount.user.Email))
                {
                    //create account
                    var accResult = await _account.CreateAsync(createFullAccount.account, accId, roleInfo.Id);
                    if (accResult == Service.Enum.RESPONSECODE.OK)
                    {
                        //create userInfo
                        var userResult = await _user.CreateAsync(createFullAccount.user, accId);
                        if (userResult == Service.Enum.RESPONSECODE.OK)
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
            return BadRequest(404);
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
                RoleViewModel roleInfo = await _role.GetRoleIdByNameAsync("User");
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
                else return BadRequest("Your login information incorect");
            }
            else
            {
                string message = string.Join(" | ", ModelState.Values
                      .SelectMany(v => v.Errors)
                      .Select(e => e.ErrorMessage));

                return BadRequest(message);
            }
        }

        //This API will excute task verify Email and send link to reset password
        [AllowAnonymous]
        [HttpPost("Forgot-Password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotpasswordViewModel forgotpassword)
        {
            var isExist = await _user.GetValidEmailAsync(forgotpassword.Email);
            if(isExist == null) return NotFound("Email not found");

            var token = _token.GenerateResetToken(isExist.Email);

            var resetUrl = $"http://127.0.0.1:5500/verify.html?token={token}"; 
            await _email.SendResetPasswordEmail(forgotpassword.Email, resetUrl);

            return Ok("Reset password link has been sent to your email");


        }


        //This API will excute task reset password
        [HttpPost("Reset-Password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewoModel request)
        {
            var isExistEmail = await _token.CheckResetTokenAsync(request.token);
            if(isExistEmail == null) return BadRequest("Token is invalid or expired");  

            var user = await _user.GetValidEmailAsync(isExistEmail.Email);
            if(user == null) return NotFound("User not found");

            var resetpassword = await _account.RessetPasswordAsync(user.AccountId, _hash.SHA256(request.NewPassword + privateKey));
            if(resetpassword)
            {
                return Ok("Password reset successfully");
            }
            else
            {
                return BadRequest("Failed to reset password");
            }
        }



    }
}
