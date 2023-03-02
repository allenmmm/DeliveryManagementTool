using DMT.GeneratingOrderBooks.Domain.ValueObjects;
using DMT.SharedKernel;
using DMT.SharedKernel.Interface;
using DMT.SharedKernel.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DMT.GeneratingOrderBooks.Domain.Entities
{
    public class Orderbook : Entity<ulong>
    {
        private readonly List<Order> _Orders = new List<Order>();
        public IEnumerable<Order> Orders => _Orders.AsReadOnly();
        public DateStamp_VO DateStamp { get; private set; }
        public int SupplierId { get; private set; }

        private Orderbook() { }

        public Orderbook(IEnumerable<PlannedAndOverdueOrder> plannedAndOverdueOrders, 
                        IDateTime dateTime )
             : base(CalculateOrderbookId(dateTime.GetTime(), 
                    plannedAndOverdueOrders.First().SupplierId))
        {
            DateStamp = DateStamp_VO.Create(plannedAndOverdueOrders.First().DatePulled,
                                            dateTime.GetTime());
            foreach (var order in plannedAndOverdueOrders)
            {
                AddOrder(new Order(order));
            }
        }

        //This function has to be public as its called by the repository and ONLY the
        //repository
        public void AddOrder(Order order)
        {
            _Orders.Add(order);
        }
        
        public static ulong CalculateOrderbookId(DateTime dateTime, int supplierId)
        {
            var currentOrderbookWeek = OrderbookWeek_VO.Create(dateTime).ToString();        
            var temp = Convert.ToString(supplierId) + currentOrderbookWeek;
            ulong value =  ulong.Parse(temp);
            return (value);
        }
    }
}
