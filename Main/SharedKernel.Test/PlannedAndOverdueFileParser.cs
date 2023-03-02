using DMT.GeneratingOrderBooks.Domain.Entities;
using SharedKernel.Test.Interfaces;
using System;
using System.Collections.Generic;

namespace SharedKernel.Test
{
    public class PlannedAndOverdueFileParser : IProcessData
    {
        private List<PlannedAndOverdueOrder> _PlannedAndOverdueOrders;
        private readonly DateTime DatePulled;

        public IEnumerable<PlannedAndOverdueOrder> PlannedAndOverdueOrders { get { return _PlannedAndOverdueOrders.AsReadOnly(); } }
 
        public PlannedAndOverdueFileParser(DateTime datePulled)
        {
            _PlannedAndOverdueOrders = new List<PlannedAndOverdueOrder>();
            DatePulled = datePulled;
        }

        public void ProcessLine(List<string> rawData)
        {
            if (rawData[0] != String.Empty)
            {
                var order_Pk = rawData[0];
                var supplier_PK = Convert.ToInt32(rawData[1]);
                var supplierName = rawData[2];
                var partNumber = rawData[3];
                var partDescription = rawData[4];
                var purchaseOrder = rawData[5];
                var poLine = Convert.ToInt32(rawData[6]);
                var poSched = Convert.ToInt32(rawData[7]);
                var openPOQty = Convert.ToInt32(rawData[8]);
                var itemDeliveryDate = DateTime.Parse(rawData[9]);
                var startDeliveryDate = DateTime.Parse(rawData[10]);
                DateTime datePulled;
                if (DatePulled == DateTime.MinValue)
                    datePulled = DateTime.ParseExact(rawData[11], "dd-MM-yyyy HH-mm-ss", System.Globalization.CultureInfo.InvariantCulture);
                else
                    datePulled = DatePulled;
                _PlannedAndOverdueOrders.Add(new
                    PlannedAndOverdueOrder(order_Pk,
                                          supplier_PK,
                                          supplierName,
                                          partNumber,
                                          partDescription,
                                          purchaseOrder,
                                          poLine,
                                          poSched,
                                          openPOQty,
                                          itemDeliveryDate,
                                          startDeliveryDate,
                                          datePulled));
            }
        }
    }
}
