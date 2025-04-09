using Azure.Core;
using FigurineFrenzeyViewModel.Bid;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Service.AuctionHubService;
using Service.AuctionService;
using Service.BidService;
using Service.TokenService;
using Service.UserService;

namespace FigurineFrenzy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BidController : ControllerBase
    {
        private readonly IBidService _bid;
        private readonly ITokenService _token;
        private readonly IAuctionService _auction;
        private readonly IHubContext<AuctionHubService> _auctionHub;
        private readonly IUserService _user;
        public BidController(IBidService bid, ITokenService token, IAuctionService auction, IHubContext<AuctionHubService> auctionHub, IUserService user)
        {
            _bid = bid;
            _token = token;
            _auction = auction;
            _auctionHub = auctionHub;
            _user = user;
        }

        [Authorize(Roles = "User")]
        [HttpPost("CreateBid")]
        public async Task<IActionResult> Create(CreateBidViewModel createBidView)
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
                        var auction = await _auction.GetAsync(createBidView.AuctionId);
                        var user = await _user.GetAsync(checkToken.AccountId);
                     
                        if (auction != null && auction.EndTime >= DateTime.Now && ValidBid(createBidView.BidAmount,auction.StepPrice,auction.CurrentPrice,auction.StepPrice))
                        {

                            var createBid = await _bid.Create(createBidView, checkToken.AccountId);
                            if (createBid == Service.Enum.RESPONSECODE.OK)
                            {
                                var updateCurrentPrice = await _auction.UpdateCurrentPriceAsync(createBidView.AuctionId, createBidView.BidAmount);
                                if (updateCurrentPrice == Service.Enum.RESPONSECODE.OK)
                                {
                                    await _auctionHub.Clients.All.SendAsync("ReceiveBidUpdate", new
                                    {
                                        AuctionId = createBidView.AuctionId,
                                        BidAmount = createBidView.BidAmount,
                                        Bidder = user.FullName
                                    });
                                    await _auctionHub.Clients.Group($"auction_{createBidView.AuctionId}")
                                        .SendAsync("UserJoinedAuction", user.FullName);
                                    //You can add the user to the group directly here if needed
                                    await _auctionHub.Groups.AddToGroupAsync(createBidView.AuctionId, $"auction_{createBidView.AuctionId}");
                                    return Ok(createBidView.BidAmount);

                                }
                                else return StatusCode(500, "Can't Update Current Price");

                            }
                            else return StatusCode(500, "Can't create Bid");
                        }
                        else return BadRequest("Invalid Bid Amount or Auction is Expired");

                    }
                    else return Unauthorized();
                }
                else return Unauthorized();
            }
            else return Unauthorized();
        }

        private bool ValidBid(double bidAmount,double? stepPrice,double? currentPrice, double? StartPrice)
        {
            if(currentPrice == null)
            {
                if (bidAmount > StartPrice && bidAmount - StartPrice >= stepPrice)
                {
                    return true;
                }
                else
                {
                    return true;
                }
            }else if (currentPrice != null)
            {
                if (bidAmount > currentPrice && bidAmount - currentPrice >= stepPrice)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }
    }
}
