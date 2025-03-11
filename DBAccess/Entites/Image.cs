using System;
using System.Collections.Generic;

namespace DBAccess.Entites;

public partial class Image
{
    public string Id { get; set; } = null!;

    public string? ImageSetId { get; set; }

    public string? ImageUrl { get; set; }

    public bool IsActive { get; set; }

    public virtual ImageSet? ImageSet { get; set; }
}
