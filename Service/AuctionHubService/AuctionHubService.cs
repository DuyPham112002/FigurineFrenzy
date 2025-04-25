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
        public async Task JoinAuctionGroup(string auctionId, string userName)
        {
            var groupName = $"auction_{auctionId}";
            

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            // Notify others already in the group (not the one who just joined)
            await Clients.OthersInGroup(groupName)
                    .SendAsync("UserJoinedAuction", auctionId, userName);
        }

        public async Task UpdateCurrentPrice(string auctionId, double currentPrice,string userName)
        {
            var groupName = $"auction_{auctionId}";
            await Clients.All.SendAsync("ReceiveBidUpdate", auctionId, currentPrice);

            //Send to people in the auction group for extra info
            await Clients.Groups(groupName)
                .SendAsync("ReceiveAuctionBidDetail", auctionId, userName);
        }

        public async Task UpdateStatus(string auctionId, string status)
        {
            await Clients.All.SendAsync("ReceiveAuctionStatus", auctionId, status);
            //Send to people in the auction group for extra info
            await Clients.Groups($"auction_{auctionId}")
                .SendAsync("ReceiveAuctionStatusDetail", auctionId, status);
        }

        public async Task NotifyNewAuctionCreated()
        {
            await Clients.All.SendAsync("ReceiveNewAuctionAlert");
        }


    }
}
