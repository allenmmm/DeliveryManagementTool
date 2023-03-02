using DMT.GeneratingOrderBooks.Domain.Entities;
using DMT.GeneratingOrderBooks.Domain.Interfaces;
using DMT.SharedKernel;
using DMT.SharedKernel.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DMT.GeneratingOrderBooks.Data
{
    public class GeneratingOrderbooksRepository : IGeneratingOrderbooksRepo, IDisposable
    {
        private readonly GeneratingOrderbooksContext _Context;

        public GeneratingOrderbooksRepository(GeneratingOrderbooksContext context)
        {
            _Context = context;
        }

        protected virtual void Dispose(bool disposing)
        {
            // then free native/unmanaged resources here
            if (_Context != null)
                _Context.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void UpdatePlannedAndOverdues(IEnumerable<PlannedAndOverdueOrder> PlannedAndOverdueOrders)
        {
            _Context.PlannedAndOverdueOrders.AddRange(PlannedAndOverdueOrders);
            _Context.SaveChanges();
        }

        public IEnumerable<int> GetDistinctSupplierCodesFromPANDO()
        {
            var plannedAndOverdues = _Context.PlannedAndOverdueOrders
                    .Select(sc => sc.SupplierId)
                    .Distinct().ToList();

            if (plannedAndOverdues.Count() == 0)
                throw new DomainException("Planned and Overdues Table is empty");

            return (plannedAndOverdues);
        }

        public IEnumerable<PlannedAndOverdueOrder> GetOrdersPerSupplierFromPANDO(int supplierCode)
        {
            return (_Context.PlannedAndOverdueOrders
                .Where(po => po.SupplierId == supplierCode))
                .ToList();
        }

        public Supplier GetSupplierWithOrderbookAndOrders(int supplierCode,
                                                 IDateTime dateTimeConfig)
        {
            var orderBookId = Orderbook.CalculateOrderbookId(dateTimeConfig.GetTime(),
                                                             supplierCode);
            if (_Context.Orderbooks.Any(ob => ob.Id == orderBookId))
            {
                return (GetSupplierWithOrderbookAndOrders(orderBookId));
            }
            return _Context.Supplier.Where(s => s.Id == supplierCode).FirstOrDefault();
        }

        public Supplier GetSupplierWithOrderbookAndOrders(ulong orderBookId)
        {
            Supplier supplier = null;

            var orderbook = _Context.Orderbooks
                      .Include(s => s.Orders)
                      .Where(ob => ob.Id == orderBookId)
                      .FirstOrDefault();

            if (orderbook != null)
            {
                var selectedSupplier = 
                        _Context.Supplier.Select(s => new { s.Id, s.Details.Name })
                                         .Where(s => s.Id == orderbook.SupplierId).First();
                supplier = new Supplier(selectedSupplier.Id, selectedSupplier.Name);
                supplier.AddOrderBook(orderbook);
            }
            return (supplier);
        }


        public Supplier GetSupplierWithOrderbooks(int supplierCode)
        {
            return _Context.Supplier.Where(supplier => supplier.Id == supplierCode)
                                .Include(supplier => supplier.Orderbooks)
                                .ThenInclude(a => a.Orders).FirstOrDefault();
        }

        public string GetSupplierNameFromPANDO(int supplierCode)
        {
            return _Context.PlannedAndOverdueOrders
                .Where(po => po.SupplierId == supplierCode).First().SupplierName;
        }

        public void ValidatePlannedAndOverDues()
        {
            var firstOrder = _Context.PlannedAndOverdueOrders.First();
            if (_Context.PlannedAndOverdueOrders
                .Where(po => po.DatePulled != firstOrder.DatePulled)
                .Count() > 0)
            {
                throw new DomainException("Date pulled not consistent throughout planned and overdues");
            }
        }

        public void DeletePlannedAndOverdues()
        {
            _Context.PlannedAndOverdueOrders.RemoveRange(_Context.PlannedAndOverdueOrders);
            _Context.SaveChanges();
        }

        public void SaveSupplier(Supplier supplier)
        {
            var existingSupplier = _Context.Supplier
                .Where(s => s.Id == supplier.Id)
                .FirstOrDefault();

            if (existingSupplier == null) // no supplier created
            {
                _Context.Add(supplier); //will add supplier, ob and o
            }
            else
            {
                if (supplier.Orderbooks != null) //then
                {
                    var newOrderbook = supplier.Orderbooks.First();
                    var existingSupplierWithOrderBookAndOrders =
                        GetSupplierWithOrderbookAndOrders(newOrderbook.Id);

                    if (existingSupplierWithOrderBookAndOrders == null) //no orderbook for this supplier is stored
                    {
                        _Context.Attach(existingSupplier);
                        existingSupplier.AddOrderBook(newOrderbook);
                    }
                    else// found an existing orderbook
                    {
                        _Context.Attach(existingSupplierWithOrderBookAndOrders);
                        var existingOrderbook = existingSupplierWithOrderBookAndOrders
                            .Orderbooks.First();
                        _Context.Entry(existingOrderbook)
                            .Reference("DateStamp").CurrentValue = newOrderbook.DateStamp;

                        foreach (var order in newOrderbook.Orders)
                        {
                            //try to find the order in db
                            var existingOrder = existingOrderbook
                                .Orders.Where(o => o.PandOId == order.PandOId).FirstOrDefault();
                            if (existingOrder == null) //couldnt find the order in the database
                            {
                                existingOrderbook.AddOrder(order);
                            }
                            else // found the order
                            {
                                _Context.Entry(existingOrder).Reference("Details").CurrentValue = order.Details;
                                _Context.Entry(existingOrder).Reference("Part").CurrentValue = order.Part;
                            }
                        }
                        //loop through the existing context and delete those in the context but not in the new supplier
                        foreach (var existingOrder in existingOrderbook.Orders)
                        {
                            if (!newOrderbook.Orders.Any(o => o.PandOId == existingOrder.PandOId)) 
                            {
                                _Context.Remove(existingOrder);
                            }
                        }
                    }
                }
            }
            _Context.SaveChanges();
        }

        public IEnumerable<String> GetOrderbookWeeks()
        {
            return _Context.Orderbooks
                .OrderByDescending(o => o.DateStamp.DateCreated)
                .Select(o => o.DateStamp.OrderbookWeek)
                       .Distinct().ToList(); 
        }

        public IEnumerable<Supplier> GetSuppliersWithOrderbookForWeek(string orderbookWeek)
        {
            List<Supplier> suppliers = new List<Supplier>();

            _Context.Orderbooks
                .Where(ob => ob.DateStamp.OrderbookWeek == orderbookWeek)
                .ToList()
                .ForEach(ob =>  {
                    var d = _Context.Supplier.Select(s => new { s.Id, s.Details.Name})
                        .Where(s => s.Id == ob.SupplierId).First();
                    var supplier = new Supplier(d.Id, d.Name);
                    supplier.AddOrderBook(ob);
                    suppliers.Add(supplier);
                });

            return (suppliers);
        }
    }
}