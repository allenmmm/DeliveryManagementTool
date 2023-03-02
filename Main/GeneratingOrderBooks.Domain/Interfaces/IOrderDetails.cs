using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.GeneratingOrderBooks.Domain.Interfaces
{
    public interface IOrderDetails
    {
        string Id { get; }
        string PurchaseOrder { get;  }
        int POLineItem { get; }
        int POSchedLine { get;  }
        int OpenPOQty { get; }
        DateTime ItemDeliveryDate { get;  }
        DateTime StatDeliverySchedule { get;  }
    }
}
