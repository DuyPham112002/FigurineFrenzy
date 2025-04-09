using FigurineFrenzeyViewModel.Image;
using FigurineFrenzeyViewModel.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Img;
using Service.TokenService;
using System.Buffers.Text;
using System.Web;

namespace FigurineFrenzy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImgService _img;
        private readonly ITokenService _token;

        public ImageController(IImgService img, ITokenService token)
        {
            _img = img;
            _token = token;
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet("GetImage")]
        public async Task<IActionResult> GetAllImage(string ImageSetId)
        {
            string header = Request.Headers["Authorization"].ToString();
            if (header != null && header.Length > 0)
            {
                string token = header.Split(" ")[1];
                if (token != null)
                {
                    var checkToken = await _token.CheckTokenAsync(token);
                    if (checkToken != null && checkToken.Role == "User" || checkToken.Role == "Admin")
                    {
                        List<InfoImageViewModel> getAllImg = await _img.GetAllAsync(ImageSetId);
                        if(getAllImg == null)                       
                            return StatusCode(400, "Can't Get List Image");
                        
                        var root = Directory.GetCurrentDirectory() + "/wwwroot/Images";
                        var hostUrl = "http://localhost:5114/";
                        var imageUrls = getAllImg
                        .Where(img => !string.IsNullOrEmpty(img.ImgUrl)) // Ensure no null URLs
                        .Select(img => new
                        {
                           
                            ImgUrl = hostUrl + img.ImgUrl
                        })
                   .ToList();
                        return Ok(imageUrls);
                    }
                    else return Unauthorized();
                }
                else return Unauthorized();
            }
            else return Unauthorized();
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet("Document/{filename}")]
        public async Task<IActionResult> GetDocument(string filename)
        {
            string header = Request.Headers["Authorization"].ToString();
            if (header != null && header != string.Empty)
            {
                string token = header.Split(" ")[1];
                if (token != null)
                {
                    var checkedToken = await _token.CheckTokenAsync(token);
                    if (checkedToken != null && (checkedToken.Role == "Admin" || checkedToken.Role == "User"))
                    {
                        string root = Directory.GetCurrentDirectory() + "/wwwroot/Images";
                        ////Decode URL to string
                        //string decodefilename = Uri.UnescapeDataString(filename);
                        ////Get only file name
                        //string fileName = System.IO.Path.GetFileName(new Uri(decodefilename).AbsolutePath);


                        string fullPath = Path.Combine(root, filename);
                        if (System.IO.File.Exists(fullPath))
                        {
                            byte[] data = await System.IO.File.ReadAllBytesAsync(fullPath);
                            string base64 = Convert.ToBase64String(data);
                            return Ok(base64);
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                    else return Unauthorized();
                }
                else return Unauthorized();
            }
            else return Unauthorized();
        }


    }
}
