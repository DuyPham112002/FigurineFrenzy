using System;
using System.Collections.Generic;

namespace DBAccess.Entites;

public partial class User
{
    public string Id { get; set; } = null!;

    public string? FullName { get; set; }

    public string? Address { get; set; }

    public string? Email { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public string? AccountId { get; set; }

    public virtual Account? Account { get; set; }
}
