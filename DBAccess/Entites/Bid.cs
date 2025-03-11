using System;
using System.Collections.Generic;

namespace DBAccess.Entites;

public partial class Bid
{
    public string Id { get; set; } = null!;

    public string AuctionId { get; set; } = null!;

    public string Bidder { get; set; } = null!;

    public double? BidAmount { get; set; }

    public DateTime? BidTime { get; set; }

    public virtual Auction Auction { get; set; } = null!;

    public virtual Account BidderNavigation { get; set; } = null!;
}
