using DMT.DTO;
using DMT.GeneratingOrderBooks.Domain.Entities;
using DMT.SharedKernel.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DMT.GeneratingOrderbooks.Service
{
    public static class DTOConversion
    {
        public static IReadOnlyList<OrderbookWeekDTO> ConvertOrderbookWeeksToOrderbookWeekDTOs
            (IEnumerable<String> orderbookWeeks)
        {
            List<OrderbookWeekDTO> orderbookWeekDTOs =
                new List<OrderbookWeekDTO>();
            foreach(var orderbookWeek in orderbookWeeks)
            {
                orderbookWeekDTOs.Add(new OrderbookWeekDTO(orderbookWeek));
            }
            return (orderbookWeekDTOs);
        }

        public static OrderbookPreviewsDTO ConvertSuppliersToOrderbookPreviewsDTO
            (IEnumerable<Supplier> suppliers)
        {
            List<OrderbookPreviewDTO> orderbookPreviewDTOs = new List<OrderbookPreviewDTO>();
            foreach (var supplier in suppliers)
            {
                foreach (var orderBook in supplier.Orderbooks)
                {
                    orderbookPreviewDTOs.Add(
                        ConvertSupplierOrderbookToOrderbookPreviewDTO(supplier, orderBook));
                }
            }

            OrderbookPreviewsDTO orderbookPreviewsDTO =
                new OrderbookPreviewsDTO(OrderbookWeek_VO.Create(orderbookPreviewDTOs.First()
                                            .DateGenerated).StartOfOrderBookWeek, orderbookPreviewDTOs);
            return (orderbookPreviewsDTO);
        }

        private static OrderbookPreviewDTO ConvertSupplierOrderbookToOrderbookPreviewDTO(Supplier supplier,
                                                                                         Orderbook orderbook)
        {
            OrderbookPreviewDTO orderbookPreviewDTO =
                new OrderbookPreviewDTO(orderbook.Id,
                                        orderbook.DateStamp.DateCreated,
                                        supplier.Id,
                                        supplier.Details.Name);
            return orderbookPreviewDTO;
        }

        public static OrderbookDTO ConvertOrderbookToOrderbookDTO(Supplier supplier)
        {
            List<OrderDTO> orderDTOs = new List<OrderDTO>();
            foreach (var order in supplier.Orderbooks.First().Orders)
            {
                orderDTOs.Add(ConvertOrderToOrderDTO(order));
            }

            OrderbookDTO orderbookDTO =
                new OrderbookDTO(supplier.Details.Name,
                                    supplier.Id,
                                    supplier.Orderbooks.First().DateStamp.OrderbookWeek,
                                    supplier.Orderbooks.First().DateStamp.DateCreated,
                                     orderDTOs);
            return orderbookDTO;
        }

        private static OrderDTO ConvertOrderToOrderDTO(Order order)
        {
            return new OrderDTO(order.Details.PurchaseOrder,
                                order.Details.POLineItem,
                                order.Details.POSchedLine,
                                order.Details.OpenPOQty,
                                order.Details.ItemDeliveryDate,
                                order.Details.StatDeliverySchedule,
                                order.Part.Number,
                                order.Part.Description);
        }
    }
}
