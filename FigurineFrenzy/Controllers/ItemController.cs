using FigurineFrenzeyViewModel.Image;
using FigurineFrenzeyViewModel.Item;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Abstractions;
using Service.AuctionService;
using Service.Img;
using Service.ImgSet;
using Service.ItemService;
using Service.TokenService;
using System.Diagnostics.Eventing.Reader;


namespace FigurineFrenzy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _item;
        private readonly ITokenService _token;
        private readonly IImgService _img;
        private readonly IImgSetService _imgSet;
        private readonly IHostEnvironment _webHostEnvironment;
        private readonly IAuctionService _auction;

        public ItemController(IItemService item, ITokenService token, IImgService img, IImgSetService imgSet, IHostEnvironment webHostEnvironment, IAuctionService auction)
        {
            _item = item;
            _token = token;
            _img = img;
            _imgSet = imgSet;
            _webHostEnvironment = webHostEnvironment;
            _auction = auction;
        }

        [Authorize(Roles = "User")]
        [HttpPost("CreateItem")]
        public async Task<IActionResult> Create(CreateItemViewModel item)
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

                        var imgSetId = Guid.NewGuid().ToString();

                        var createImgSet = await _imgSet.CreateAsync(imgSetId);
                        if (createImgSet == Service.Enum.RESPONSECODE.OK)
                        {
                            string[] permittedExtensions = { ".png", ".jpg", ".jepg" };

                            //check image
                            foreach (IFormFile image in item.Files)
                            {
                                //check signature file
                                using (var memoryStream = new MemoryStream())
                                {
                                    //copy list file into memoryStream
                                    await image.CopyToAsync(memoryStream);

                                    string filenames = Path.GetFileName(image.FileName);

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
                                var ext = Path.GetExtension(image.FileName).ToLowerInvariant();
                                var filename = Path.GetRandomFileName();
                                filename = filename.Split('.')[0];
                                string fileNameWithPath = Path.Combine(path, filename + ext);

                                //save img to Server

                                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                                {
                                    image.CopyTo(stream);
                                }
                                string imageUrl = "Images/" + filename + ext;
                                //save img metadata to db
                                bool imageSaved = await _img.CreateImageBase(new ImageViewModel()
                                {
                                    ImageSetId = imgSetId,
                                    ImageUrl = imageUrl
                                });


                                if (!imageSaved)
                                {
                                    return BadRequest();
                                }


                            }
                            var creatItem = await _item.CreateAsync(item, imgSetId);
                            if (creatItem == Service.Enum.RESPONSECODE.OK)
                            {
                                return Ok("Create Item Success");
                            }
                            else
                            {
                                //if create item failed, delete imgSet have already created in database
                                var deleteImgSet = await _imgSet.DeleteAsync(imgSetId);
                                return BadRequest("Cannot Create Item to Add ");
                            }

                        }
                        else
                        {
                            //If create imgSet failed, delete Auction have already created in database
                            var deleteAuction = await _auction.DeleteAsync(checkToken.AccountId,item.AuctionId);
                            return BadRequest("Cannot Create ImageSet");
                        }

                       

                    }
                    else return Unauthorized();
                }
                else return Unauthorized();
            }
            else return Unauthorized();
        }

        [Authorize(Roles ="User,Admin")]
        [HttpGet("GetAllByAuctionId")]
        public async Task<IActionResult> GetAllByAuctionId(string auctionId)
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
                        List<GetinfoItemViewModel> item = await _item.GetAllAsync(auctionId);
                        if (item != null && item.Count > 0)
                        {
                            return Ok(item);
                        }
                        else return StatusCode(400, "Can't Get List Item");
                    }else return Unauthorized();
                }else return Unauthorized();
            }else return Unauthorized();

        }


        [Authorize(Roles = "User")]
        [HttpPost("UploadImage")]
        [RequestSizeLimit(2 * 1024 * 1024)] // limit 2mb
        [RequestFormLimits(MultipartBodyLengthLimit = 2 * 1024 * 1024)]
        public async Task<IActionResult> UploadImage(List<IFormFile> files, [FromForm] string imgSetId)
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
                        string[] permittedExtensions = { ".png", ".jpg", ".jepg" };

                        //check image
                        foreach (IFormFile image in files)
                        {
                            //check signature file
                            using (var memoryStream = new MemoryStream())
                            {
                                //copy list file into memoryStream
                                await image.CopyToAsync(memoryStream);

                                string filenames = Path.GetFileName(image.FileName);

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
                            var ext = Path.GetExtension(image.FileName).ToLowerInvariant();
                            var filename = Path.GetRandomFileName();
                            filename = Path.GetRandomFileName();
                            string fileNameWithPath = Path.Combine(path, filename);

                            //save img to Server
                            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                            {
                                image.CopyTo(stream);
                            }
                            string imageUrl = "Images/" + filename;
                            //save img metadata to db
                            bool imageSaved = await _img.CreateImageBase(new ImageViewModel()
                            {
                                ImageSetId = imgSetId,
                                ImageUrl = imageUrl
                            });

                            if (!imageSaved)
                            {
                                return BadRequest();
                            }

                        }
                        return Ok();
                    }
                    else return Unauthorized();
                }
                else return Unauthorized();
            }
            else return Unauthorized();
        }
    }
}
