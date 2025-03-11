using System;
using System.Collections.Generic;

namespace DBAccess.Entites;

public partial class Account
{
    public string AccountId { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string RoleId { get; set; } = null!;

    public DateTime CreateAt { get; set; }

    public bool IsActive { get; set; }

    public double Balance { get; set; }

    public virtual ICollection<Admin> Admins { get; set; } = new List<Admin>();

    public virtual ICollection<Auction> AuctionOwners { get; set; } = new List<Auction>();

    public virtual ICollection<Auction> AuctionWinners { get; set; } = new List<Auction>();

    public virtual ICollection<Bid> Bids { get; set; } = new List<Bid>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<Token> Tokens { get; set; } = new List<Token>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
