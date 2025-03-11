using System;
using System.Collections.Generic;

namespace DBAccess.Entites;

public partial class Item
{
    public string ItemId { get; set; } = null!;

    public string NameOfProduct { get; set; } = null!;

    public string? Description { get; set; }

    public string AuctionId { get; set; } = null!;

    public string ImageSetId { get; set; } = null!;

    public virtual Auction Auction { get; set; } = null!;

    public virtual ImageSet ImageSet { get; set; } = null!;
}
