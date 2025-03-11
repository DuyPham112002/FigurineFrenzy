using System;
using System.Collections.Generic;

namespace DBAccess.Entites;

public partial class Role
{
    public string RoleId { get; set; } = null!;

    public string? Name { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}
