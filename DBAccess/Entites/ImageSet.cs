using System;
using System.Collections.Generic;

namespace DBAccess.Entites;

public partial class ImageSet
{
    public string Id { get; set; } = null!;

    public bool? IsActive { get; set; }

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}
