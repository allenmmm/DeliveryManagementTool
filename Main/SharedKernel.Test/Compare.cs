using DMT.GeneratingOrderBooks.Domain.Entities;
using DMT.GeneratingOrderBooks.Domain.Interfaces;
using DMT.GeneratingOrderBooks.Domain.ValueObjects;
using DMT.SharedKernel.Interface;
using DMT.SharedKernel.ValueObjects;
using FluentAssertions;
using FluentAssertions.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SharedKernel.Test
{
    public static class Compare
    {
        public static void Collection<T>(IEnumerable<T> list_ACT,
                                       IEnumerable<T> list_EXP)
        {
            try
            {
                list_ACT.Count().Should().Be(list_EXP.Count());
                var enum_ACT = list_ACT.GetEnumerator();
                var enum_EXP = list_EXP.GetEnumerator();
                enum_ACT.MoveNext();
                while (enum_EXP.MoveNext() == true)
                {
                    CompareItem(enum_ACT.Current, enum_EXP.Current);
                    enum_ACT.MoveNext();
                }
            }
            catch
            {
                if (list_EXP != null)
                    throw;
                list_ACT.Should().BeNull();
            }
        }

        public static void CompareItem<T>(T item_ACT, T item_EXP)
        {
            var act = item_ACT.GetType();
            var exp = item_EXP.GetType();
          //  item_ACT.Should().Be(item_EXP);
            var actFields = act.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (FieldInfo field in actFields)
            {
                object value_ACT = field.GetValue(item_ACT);
                object value_EXP = field.GetValue(item_EXP);
                if (value_EXP == null)
                {
                    value_ACT.Should().BeNull();
                }
                value_ACT.Should().Be(value_EXP);
            }
        }

        public static void OrderDetailsWithPandO(Order_VO order,
                                 IOrderDetails orderPandO)
        {

            order.ItemDeliveryDate.Should().Be(orderPandO.ItemDeliveryDate);
            order.OpenPOQty.Should().Be(orderPandO.OpenPOQty);
            order.POLineItem.Should().Be(orderPandO.POLineItem);
            order.POSchedLine.Should().Be(orderPandO.POSchedLine);
            order.PurchaseOrder.Should().Be(orderPandO.PurchaseOrder);
            order.StatDeliverySchedule.Should().Be(orderPandO.StatDeliverySchedule);
        }

        public static void PartWithPandO(Part_VO part,
                                IPart orderPandO)
        {
            part.Number.Should().Be(orderPandO.PartNumber);
            part.Description.Should().Be(orderPandO.PartDescription);
        }

        public static void SupplierDetailsWithPandO(Supplier_VO supplier,
                                             ISupplier orderPandO)
        {
            supplier.Name.Should().Be(orderPandO.SupplierName);
        }

        public static void OrderbookWithPandO(Orderbook orderBook, 
                                             IEnumerable<PlannedAndOverdueOrder> ordersPandO,
                                             IDateTime dateTimeConfig)
        {
            var supplierId_EXP = ordersPandO.First().SupplierId;
            var orderbookId_EXP = Orderbook.CalculateOrderbookId(dateTimeConfig.GetTime(), 
                                                                    supplierId_EXP);
            orderBook.Id.Should().Be(orderbookId_EXP); 
            orderBook.DateStamp.DateCreated.Should()
                .BeWithin(20.Seconds()).After(dateTimeConfig.GetTime());
            string orderbookWeek_EXP = OrderbookWeek_VO.Create(dateTimeConfig.GetTime())
                                        .FormatString();
            int i = 0;
            orderBook.Orders.Count().Should().Be(ordersPandO.Count());
            foreach (var order_ACT in orderBook.Orders)
            {
                orderBook.DateStamp.OrderbookWeek.Should().Be(orderbookWeek_EXP);
                //This conditional is required because TSQL SmallDateTime has an accuracy of 1 minute
                if (ordersPandO.ElementAt(i).DatePulled > orderBook.DateStamp.DatePulled)
                {
                    orderBook.DateStamp.DatePulled.Should().BeWithin(1.Minutes())
                        .Before(ordersPandO.ElementAt(i).DatePulled);
                }
                else
                {
                    orderBook.DateStamp.DatePulled.Should()
                        .BeWithin(1.Minutes()).After(ordersPandO.ElementAt(i).DatePulled);
                }
                order_ACT.PandOId.Should().Be(ordersPandO.ElementAt(i).Id);
                Compare.OrderDetailsWithPandO(order_ACT.Details, ordersPandO.ElementAt(i));
                Compare.PartWithPandO(order_ACT.Part, ordersPandO.ElementAt(i++));
            }
        }

        public static void SupplierWithPandO(Supplier supplier, PlannedAndOverdueOrder orderPandO)
        {
            supplier.Id.Should().Be(orderPandO.SupplierId);
            Compare.SupplierDetailsWithPandO(supplier.Details, orderPandO);
        }
    }
}