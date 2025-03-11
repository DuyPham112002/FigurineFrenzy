using System;
using System.Collections.Generic;

namespace DBAccess.Entites;

public partial class Token
{
    public string TokenId { get; set; } = null!;

    public string Value { get; set; } = null!;

    public bool IsActive { get; set; }

    public string AccountId { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public virtual Account Account { get; set; } = null!;
}
