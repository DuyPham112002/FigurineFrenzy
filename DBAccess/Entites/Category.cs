using System;
using System.Collections.Generic;

namespace DBAccess.Entites;

public partial class Category
{
    public string Id { get; set; } = null!;

    public string? Name { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Auction> Auctions { get; set; } = new List<Auction>();
}
