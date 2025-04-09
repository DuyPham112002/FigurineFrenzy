using DBAccess.Entites;
using FigurineFrenzeyViewModel.Auction;
using FigurineFrenzeyViewModel.Item;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Service.AuctionHubService;
using Service.AuctionService;
using Service.Img;
using Service.ItemService;
using Service.TokenService;

namespace FigurineFrenzy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionController : ControllerBase
    {
        private readonly ITokenService _token;
        private readonly IAuctionService _auction;
        private readonly IItemService _item;
        private readonly IImgService _img;
        private readonly IHubContext<AuctionHubService> _auctionhub;
        public AuctionController (ITokenService token, IAuctionService auction,IItemService item,IImgService img, IHubContext<AuctionHubService> auctionhub)
        {
            _token = token;
            _auction = auction;
            _item = item;
            _img = img;
            _auctionhub = auctionhub;
        }

        [Authorize(Roles = "User")]
        [HttpPost("CreateAuction")]
        public async Task<IActionResult> Create(string categoryId,[FromBody] CreateAuctionViewModel createAuctionView)
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
                        var createAuction = await _auction.CreateAsync(checkToken.AccountId, createAuctionView, categoryId);
                        if (createAuction != null)
                        {
                            return Ok(createAuction.AuctionId);
                        }
                        else return StatusCode(500, "Can't Create Auction");
                    }
                    else return Unauthorized();
                }
                else return Unauthorized();
            }
            else return Unauthorized();
        }

        [Authorize(Roles = "User")]
        [HttpGet("GetAllAuctionByAccId")]
        public async Task<IActionResult> GetAllByAccId()
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
                        List<Auction> listAuction = await _auction.GetAllByAccIdAsync(checkToken.AccountId);
                        if (listAuction != null)
                        {
                            return Ok(listAuction);
                        }
                        else return StatusCode(400, "Can't Get List Aution");
                    }
                    else return Unauthorized();
                }
                else return Unauthorized();
            }
            else return Unauthorized();
        }


        [Authorize(Roles = "Admin,User")]
        [HttpGet("GetAllAuction")]
        public async Task<IActionResult> GetAll()
        {
            string header = Request.Headers["Authorization"].ToString();
            if (header != null && header.Length > 0)
            {
                string token = header.Split(" ")[1];
                if (token != null)
                {
                    var checkToken = await _token.CheckTokenAsync(token);
                    if (checkToken != null && checkToken.Role == "Admin" || checkToken.Role == "User")
                    {
                        List<GetInfroAuctionViewModel> listAuction = await _auction.GetAllLiveAuctionAsync();
                        
                        if (listAuction != null)
                        {
                           return Ok(listAuction);

                        }
                        else return StatusCode(400, "Can't Get List Aution");
                    }
                    else return Unauthorized();
                }
                else return Unauthorized();
            }
            else return Unauthorized();
        }


        [Authorize(Roles = "Admin,User")]
        [HttpGet("GetAuction")]
        public async Task<IActionResult> Get([FromHeader] string AuctionId)
        {
            string header = Request.Headers["Authorization"].ToString();
            if (header != null && header.Length > 0)
            {
                string token = header.Split(" ")[1];
                if (token != null)
                {
                    var checkToken = await _token.CheckTokenAsync(token);
                    if (checkToken != null && checkToken.Role == "Admin" || checkToken.Role == "User")
                    {
                        GetInfroAuctionViewModel auction = await _auction.GetAsync(AuctionId);

                        if (auction != null)
                        {
                            return Ok(auction);

                        }
                        else return StatusCode(400, "Can't Get Aution");
                    }
                    else return Unauthorized();
                }
                else return Unauthorized();
            }
            else return Unauthorized();
        }


        [Authorize(Roles = "Admin,User")]
        [HttpPut("Completed")]
        public async Task<IActionResult> CompletedAuction(string AuctionId)
        {
            string header = Request.Headers["Authorization"].ToString();
            if (header != null && header.Length > 0)
            {
                string token = header.Split(" ")[1];
                if (token != null)
                {
                    var checkToken = await _token.CheckTokenAsync(token);
                    if (checkToken != null && checkToken.Role == "Admin" || checkToken.Role == "User")
                    {
                        var result = await _auction.CompletedAsync(AuctionId);  
                        if(result == "Live" || result == "Ended" )
                        {
                            await _auctionhub.Clients.All.SendAsync("ReceiveAuctionUpdate", new
                            {
                                AuctionId = AuctionId,
                                Status = result
                            });
                            return Ok(result);
                        }
                        else return StatusCode(400, "Auction is still lauching");
                    }else return Unauthorized();
                }else return Unauthorized();
            }else return Unauthorized();
        }


        [Authorize(Roles = "User")]
        [HttpDelete("DeleteAuction")]
        public async Task<IActionResult> Delete(string auctionId)
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
                        var result = await _auction.DeleteAsync(checkToken.AccountId, auctionId);
                        if (result == Service.Enum.RESPONSECODE.OK)
                        {
                            return Ok("Delete Auction Success");
                        }
                        else return StatusCode(400, "Can't Delete Auction");
                    }
                    else return Unauthorized();
                }
                else return Unauthorized();
            }
            else return Unauthorized();
        }
    }
}
