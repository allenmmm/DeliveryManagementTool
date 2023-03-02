using DMT.GeneratingOrderBooks.Domain.ValueObjects;
using DMT.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using DMT.SharedKernel.Interface;

namespace DMT.GeneratingOrderBooks.Domain.Entities
{
    public class Supplier : Entity<int>
    {
        public Supplier_VO Details { get; private set; }

        private readonly List<Orderbook> _Orderbooks = new List<Orderbook>();
        public IEnumerable<Orderbook> Orderbooks { get { return _Orderbooks.AsReadOnly(); } }
        private Supplier() { }

        public Supplier(int code, string name) : base(code)
        {
            Details = Supplier_VO.Create(name);
        }

        public void UpdateOrderbook(IEnumerable<PlannedAndOverdueOrder> plannedAndOverdueOrders,
                                    IDateTime dateTimeConfig)
        {
            //see if existing orderbook
            var orderBookWeek = Orderbook.CalculateOrderbookId(dateTimeConfig.GetTime(),
                                                        plannedAndOverdueOrders.First().SupplierId);
            var existingOrderbook = _Orderbooks.Where(ob =>ob.Id == orderBookWeek).FirstOrDefault();
            if (existingOrderbook != null)
            {
                CheckIfNewOrderBookIsOlderThanExisting(existingOrderbook.DateStamp.DatePulled,
                    plannedAndOverdueOrders.First().DatePulled);
                _Orderbooks.Remove(existingOrderbook);
            }
            AddOrderBook(new Orderbook(plannedAndOverdueOrders, dateTimeConfig));           
        }

        private void CheckIfNewOrderBookIsOlderThanExisting(DateTime existingDateTime, DateTime newDateTime)
        {
            if (existingDateTime > newDateTime)
            {
                throw new DomainException("New orderbook is older than existing for the given order book week");
            }
        }

        public void AddOrderBook(Orderbook orderBook)
        {
            _Orderbooks.Add(orderBook);
        }
    }
}
