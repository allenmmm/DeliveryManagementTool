using DMT.GeneratingOrderBooks.Domain.Interfaces;
using DMT.SharedKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.GeneratingOrderBooks.Domain.ValueObjects
{
    public class Order_VO : ValueObject<Order_VO>
    {
        public string PurchaseOrder { get; private set; }
        public int POLineItem { get; private set; }
        public int POSchedLine { get; private set; }
        public int OpenPOQty { get; private set; }
        public DateTime ItemDeliveryDate { get; private set; }
        public DateTime StatDeliverySchedule { get; private set; }

        private Order_VO(){}
        private Order_VO(IOrderDetails orderDetails)
        {
            PurchaseOrder = orderDetails.PurchaseOrder;
            POLineItem = orderDetails.POLineItem;
            POSchedLine = orderDetails.POSchedLine;
            OpenPOQty = orderDetails.OpenPOQty;
            ItemDeliveryDate = orderDetails.ItemDeliveryDate;
            StatDeliverySchedule = orderDetails.StatDeliverySchedule;
        }

        public static Order_VO Create(IOrderDetails orderDetails)
        {
            Gaurd.AgainstNull(orderDetails, "order value object is null");
            Gaurd.AgainstNull(orderDetails.PurchaseOrder, "Purchase order is null");
            return new Order_VO(orderDetails); 
        }
    }
}
