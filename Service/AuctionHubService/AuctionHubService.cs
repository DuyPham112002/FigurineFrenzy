using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.AuctionHubService
{
    public class AuctionHubService :Hub
    {
        public async Task JoinAuctionGroup(string auctionId, string username)
        {
            var groupName = $"auction_{auctionId}";
            

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            // Notify others already in the group (not the one who just joined)
            await Clients.OthersInGroup(groupName)
                    .SendAsync("UserJoinedAuction", auctionId, username);
        }

        public async Task UpdateCurrentPrice(string auctionId, double currentPrice)
        {
            await Clients.All.SendAsync("ReceiveBidUpdate", auctionId, currentPrice);

            //Send to people in the auction group for extra info
            await Clients.Groups($"auction_{auctionId}")
                .SendAsync("ReceiveAuctionBidDetail", auctionId, currentPrice);
        }

        public async Task UpdateStatus(string auctionId, string status)
        {
            await Clients.All.SendAsync("ReceiveAuctionStatus", auctionId, status);
            //Send to people in the auction group for extra info
            await Clients.Groups($"auction_{auctionId}")
                .SendAsync("ReceiveAuctionStatusDetail", auctionId, status);
        }


    }
}
