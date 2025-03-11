using System;
using System.Collections.Generic;

namespace DBAccess.Entites;

public partial class Admin
{
    public string Id { get; set; } = null!;

    public string? Email { get; set; }

    public string? AccountId { get; set; }

    public virtual Account? Account { get; set; }
}
