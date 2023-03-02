using DMT.GeneratingOrderBooks.Domain.Interfaces;
using DMT.GeneratingOrderBooks.Domain.ValueObjects;
using DMT.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.GeneratingOrderBooks.Domain.Entities
{
    public class PlannedAndOverdueOrder : Entity<string>, IOrderDetails, ISupplier, IPart
    {
        public int SupplierId { get; private set; }
        public string SupplierName { get; private set; }
        public string PartNumber { get; private set; }
        public string PartDescription { get; private set; }
        public string PurchaseOrder { get; private set; }
        public int POLineItem { get; private set; }
        public int POSchedLine { get; private set; }
        public int OpenPOQty { get; private set; }
        public DateTime ItemDeliveryDate { get; private set; }
        public DateTime StatDeliverySchedule { get; private set; }
        public DateTime DatePulled { get; private set; }

        private PlannedAndOverdueOrder(){}

        public PlannedAndOverdueOrder(string order_Pk,
                                      int supplierCode,
                                      string supplierName,
                                      string partNumber,
                                      string partDescription,
                                      string purchaseOrder,
                                      int poLine,
                                      int poSched,
                                      int openPOQty,
                                      DateTime itemDeliveryDate,
                                      DateTime statDeliverySchedule,
                                      DateTime datePulled) : base(order_Pk)

        {            
            SupplierId = supplierCode;
            SupplierName = supplierName;
            PartNumber = partNumber;
            PartDescription = partDescription;
            PurchaseOrder = purchaseOrder;
            POLineItem = poLine;
            POSchedLine = poSched;
            OpenPOQty = openPOQty;
            ItemDeliveryDate = itemDeliveryDate;
            StatDeliverySchedule = statDeliverySchedule;
            DatePulled = datePulled;
        }                    
    }
}
