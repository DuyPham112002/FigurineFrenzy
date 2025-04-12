using DBAccess.Entites;
using FigurineFrenzeyViewModel.User;
using FigurineFrenzy.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.TokenService;
using Service.UserService;
using static System.Net.Mime.MediaTypeNames;
using System.IO;
using FigurineFrenzeyViewModel.Image;
using Service.AccountService;

namespace FigurineFrenzy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _user;
        private readonly ITokenService _token;
        private readonly IAccountService _account;

        public UserController(IUserService user, ITokenService token, IAccountService account)
        {
            _user = user;
            _token = token;
            _account = account;
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
                        var imgProfile = await _account.GetImgAsync(checkToken.AccountId);
                        UserInfoViewModel userInfo = new UserInfoViewModel
                        {
                            FullName = getUser.FullName,
                            Address = getUser.Address,
                            DateOfBirth = getUser.DateOfBirth.Value,
                            Email = getUser.Email,
                            Phone = checkToken.Phone,
                            ImgUrl = imgProfile

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
            }
            return Unauthorized();

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

        [Authorize(Roles = "User")]
        [HttpPut("ChangeImageProfile")]
        public async Task<IActionResult> UpdateImageProfile(IFormFile file)
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
                        var isExistImage = await _account.GetImgAsync(checkToken.AccountId);
                        if (isExistImage != null)
                        {
                            //Case if avatar exist removed it
                            var existFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/", isExistImage);

                            if(System.IO.File.Exists(existFilePath))
                                System.IO.File.Delete(existFilePath);
                            

                            string[] permittedExtensions = { ".png", ".jpg", ".jepg" };
                            using (var memoryStream = new MemoryStream())
                            {
                                //copy list file into memoryStream
                                await file.CopyToAsync(memoryStream);

                                string filenames = Path.GetFileName(file.FileName);

                                //check file extension and signature
                                // Check the content length in case the file's only
                                // content was a BOM and the content is actually
                                // empty after removing the BOM.

                                if (memoryStream.Length == 0)
                                {
                                    return StatusCode(404, "Not found any image");
                                }

                                if (!FileHelper.IsValidFileExtensionAndSignature(filenames, memoryStream, permittedExtensions))
                                {
                                    return StatusCode(404, "Only upload file have extentions such as png, jpg, jepg");
                                }

                            }

                            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images");
                            //create folder if not exist
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }

                            //change filename
                            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                            var filename = Path.GetRandomFileName();
                            filename = Path.GetRandomFileName();
                            string fileNameWithPath = Path.Combine(path, filename + ext);

                            //save img to Server
                            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                            {
                                file.CopyTo(stream);
                            }
                            string imageUrl = "Images/" + filename + ext;
                            //save img metadata to db
                            bool imageSaved = await _account.UpdateImgAsync(imageUrl, checkToken.AccountId);

                            if (!imageSaved)
                            {
                                return BadRequest();
                            }
                        }
                        return Ok();
                        
                    }return Unauthorized();
                }
                else return Unauthorized();
            }
            else return Unauthorized();
        }
           
        
    }

}

