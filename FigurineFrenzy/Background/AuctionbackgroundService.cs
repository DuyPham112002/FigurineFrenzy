using Microsoft.AspNetCore.SignalR;
using Service.AuctionHubService;
using Service.AuctionService;

namespace FigurineFrenzy.Background
{
    public class AuctionbackgroundService : BackgroundService
    {

        private readonly IHubContext<AuctionHubService> _auctionHub;
        private readonly IAuctionService _auction;

        public AuctionbackgroundService(IHubContext<AuctionHubService> auctionHub, IAuctionService auction)
        {
            _auctionHub = auctionHub;
            _auction = auction;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var auctions = await _auction.GetAllAsync();
                foreach (var auction in auctions)
                {
                    if (auction.Status == "NotStart" || auction.Status == "Live")
                    {

                        var endedAuction = await _auction.CompletedAsync(auction.AuctionId);
                        if (endedAuction == "Ended")
                        {
                            // Broadcast to clients
                            await _auctionHub.Clients.All.SendAsync("ReceiveAuctionStatus", auction.AuctionId, endedAuction);
                            
                        }
                        var startAuction = await _auction.StartAsync(auction.AuctionId);

                        if (startAuction == "Live")
                        {
                            //Broadcast to clients
                            await _auctionHub.Clients.All.SendAsync("ReceiveAuctionStatus", auction.AuctionId, startAuction);
                           
                        }
                    }
                }
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
