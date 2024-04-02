using System;
using System.Collections.Generic;

namespace ConsoleApp.Data.EfCore;

public partial class InventoryTransactionType
{
    public sbyte Id { get; set; }

    public string TypeName { get; set; }

    public virtual ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();
}
