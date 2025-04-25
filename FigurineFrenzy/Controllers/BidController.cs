using Azure.Core;
using DBAccess.Entites;
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
using System.Security.Cryptography;

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
        [HttpGet("GetAllByAccId")]
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
                        List<string> bids = await _bid.GetAllAsyncById(checkToken.AccountId);
                        //loop to add user have already join Auction to group
                        foreach (var bid in bids)
                        {
                            //You can add the user to the group directly here if needed
                            await _auctionHub.Groups.AddToGroupAsync(bid, $"auction_{bid}");

                        }
                        return Ok(bids);

                    }
                    else return Unauthorized();
                }
                else return Unauthorized();
            }
            else return Unauthorized();
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
                        if (!ValidBid(createBidView.BidAmount, auction.StepPrice, auction.CurrentPrice, auction.StepPrice))
                            return StatusCode(500, $"Invalid Amount Because Steprice is {auction.StepPrice}");
                        if (auction != null && auction.EndTime >= DateTime.Now && auction.StartTime <= DateTime.Now )
                        {

                            var createBid = await _bid.CreateAsync(createBidView, checkToken.AccountId);
                            if (createBid == Service.Enum.RESPONSECODE.OK)
                            {
                                var updateCurrentPrice = await _auction.UpdateCurrentPriceAsync(createBidView.AuctionId, createBidView.BidAmount);
                                if (updateCurrentPrice == Service.Enum.RESPONSECODE.OK)
                                {
                                    await _auctionHub.Clients.All.SendAsync("ReceiveBidUpdate", new
                                    {
                                        AuctionId = createBidView.AuctionId,
                                        BidAmount = createBidView.BidAmount,
                                    });
                                    var groupName = $"auction_{createBidView.AuctionId}";
                                    await _auctionHub.Clients.Groups(groupName).SendAsync("ReceiveAuctionBidDetail", createBidView.AuctionId, user.FullName);

                             
                                    return Ok(createBidView.BidAmount);

                                }
                                else return StatusCode(500, "Can't Update Current Price");

                            }
                            else return StatusCode(500, "Can't create Bid");
                        }
                        else return BadRequest("Invalid Bid Amount or Auction is Expired or Auction Not Start Yet");

                    }
                    else return Unauthorized();
                }
                else return Unauthorized();
            }
            else return Unauthorized();
        }

        private bool ValidBid(double bidAmount, double? stepPrice, double? currentPrice, double? StartPrice)
        {
            if (currentPrice == null)
            {
                if (bidAmount > StartPrice && bidAmount - StartPrice >= stepPrice)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (currentPrice != null)
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
        [Authorize(Roles = "User")]
        [HttpGet("JoinToAuction")]
        public async Task<IActionResult> ActionResult()
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
                        var isJoined = await _bid.GetAllAsyncById(checkToken.AccountId);
                        var auction = await _auction.AuctionStatusCheckAsync(isJoined);
                        var userName = await _user.GetAsync(checkToken.AccountId);
                        if (isJoined != null && isJoined.Count() > 0)
                        {
                            foreach (var item in auction)
                            {
                                await _auctionHub.Clients.Groups($"auction_{item.AuctionId}").SendAsync("ReceiveAuctionBidDetail", item.AuctionId, userName.FullName);
                                var bidInfo = new BidInformationViewModel
                                {
                                    AuctionId = item.AuctionId,
                                    Amount = item.CurrentPrice,
                                    FullName = userName.FullName,

                                };
                                return Ok(bidInfo);  
                            }
                            return Ok("You have not joined any auction yet");
                        }
                        else
                        {
                            return BadRequest("You have not joined this auction");
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
