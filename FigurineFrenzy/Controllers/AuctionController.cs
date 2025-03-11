using DBAccess.Entites;
using FigurineFrenzeyViewModel.Auction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.AuctionService;
using Service.TokenService;

namespace FigurineFrenzy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionController : ControllerBase
    {
        private readonly ITokenService _token;
        private readonly IAuctionService _auction;

        public AuctionController (ITokenService token, IAuctionService auction)
        {
            _token = token;
            _auction = auction;
        }

        [Authorize(Roles = "User")]
        [HttpPost("CreateAuction")]
        public async Task<IActionResult> Create(string categoryId, CreateAuctionViewModel createAuctionView)
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
                    if (checkToken != null && checkToken.Role == "User")
                    {
                        List<Auction> listAuction = await _auction.GetAllAsync(checkToken.AccountId);
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
    }
}
