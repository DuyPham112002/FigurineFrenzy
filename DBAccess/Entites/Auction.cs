using System;
using System.Collections.Generic;

namespace DBAccess.Entites;

public partial class Auction
{
    public string AuctionId { get; set; } = null!;

    public double? StarPrice { get; set; }

    public double? CurrentPrice { get; set; }

    public double? FinalizePrice { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public string? Status { get; set; }

    public string? WinnerId { get; set; }

    public string OwnerId { get; set; } = null!;

    public string? CategoryId { get; set; }

    public DateTime? CreateAt { get; set; }

    public virtual ICollection<Bid> Bids { get; set; } = new List<Bid>();

    public virtual Category? Category { get; set; }

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    public virtual Account Owner { get; set; } = null!;

    public virtual Account? Winner { get; set; }
}
