using System;
using System.Collections.Generic;

namespace ConsoleApp.Data.EfCore;

public partial class OrdersTaxStatus
{
    public sbyte Id { get; set; }

    public string TaxStatusName { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
