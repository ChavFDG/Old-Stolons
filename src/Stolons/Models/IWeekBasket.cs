using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stolons.Models
{
    public interface IWeekBasket
    {
        Guid Id { get; set; }
        Consumer Consumer { get; set; }
        List<BillEntry> Products { get; set; }
    }
}
