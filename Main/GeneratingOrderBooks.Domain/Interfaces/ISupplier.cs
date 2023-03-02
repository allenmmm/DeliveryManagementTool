using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.GeneratingOrderBooks.Domain.Interfaces
{
    public interface ISupplier
    {
        int SupplierId { get; }
        string SupplierName { get; }
        DateTime DatePulled { get; }
    }
}
