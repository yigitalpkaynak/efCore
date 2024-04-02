using System;
using System.Collections.Generic;

namespace ConsoleApp.Data.EfCore;

public partial class OrdersStatus
{
    public sbyte Id { get; set; }

    public string StatusName { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
