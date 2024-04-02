using System;
using System.Collections.Generic;

namespace ConsoleApp.Data.EfCore;

public partial class OrderDetailsStatus
{
    public int Id { get; set; }

    public string StatusName { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
