using System;

namespace DMT.DTO
{
    public class OrderDTO
    {
        public string PurchaseOrder { get; private set; }
        public int POLineItem { get; private set; }
        public int POSchedLine { get; private set; }
        public int OpenPOQty { get; private set; }
        public DateTime ItemDeliveryDate { get; private set; }
        public DateTime StatDeliverySchedule { get; private set; }
        public string Number { get; private set; }
        public string Description { get; private set; }

        public OrderDTO(string purchaseOrder,
                        int pOLineItem,
                        int pOSchedLine,
                        int openPOQty,
                        DateTime itemDeliveryDate,
                        DateTime statDeliverySchedule,
                        string number,
                        string description)
        {
            PurchaseOrder = purchaseOrder;
            POLineItem = pOLineItem;
            POSchedLine = pOSchedLine;
            OpenPOQty = openPOQty;
            ItemDeliveryDate = itemDeliveryDate;
            StatDeliverySchedule = statDeliverySchedule;
            Number = number;
            Description = description;
        }

        public OrderDTO(OrderDTO orderDTO)
        {
            PurchaseOrder = orderDTO.PurchaseOrder;
            POLineItem = orderDTO.POLineItem;
            POSchedLine = orderDTO.POSchedLine;
            OpenPOQty = orderDTO.OpenPOQty;
            ItemDeliveryDate = orderDTO.ItemDeliveryDate;
            StatDeliverySchedule = orderDTO.StatDeliverySchedule;
            Number = orderDTO.Number;
            Description = orderDTO.Description;
        }
    }
}
