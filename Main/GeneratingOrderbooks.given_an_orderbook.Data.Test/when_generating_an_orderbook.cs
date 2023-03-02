using DMT.GeneratingOrderBooks.Data;
using DMT.GeneratingOrderBooks.Domain.Entities;
using DMT.SharedKernel;
using DMT.SharedKernel.Interface;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SharedKernel.Test;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneratingOrderbooks.given_an_orderbook.Data.Test
{
    [TestFixture]
    public class when_generating_an_orderbook : TestBase
    {
        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
        }
        // TODO UP TO Write a load of tests for updating plan
        // i.e. supplier name too long

        [Test]
        public void then_register_new_supplier()
        {
            //ARRANGE  
            var pando_EXP = fileParser.PlannedAndOverdueOrders.First();
            
            using (var repo_SUT = new GeneratingOrderbooksRepository(new GeneratingOrderbooksContext
                                                                        (new DbContextOptionsBuilder<DbContext>()
                                                                            .UseInMemoryDatabase(databaseName: conn).Options)))
            {
                Supplier supplier = new Supplier(pando_EXP.SupplierId, pando_EXP.SupplierName);
                //ACT
                repo_SUT.SaveSupplier(supplier);
            }
            //ASSERT
            using (var repo_SUT = new GeneratingOrderbooksRepository(new GeneratingOrderbooksContext
                                                            (new DbContextOptionsBuilder<DbContext>()
                                                                .UseInMemoryDatabase(databaseName: conn).Options)))
            {
                var supplier_ACT = repo_SUT.GetSupplierWithOrderbookAndOrders(pando_EXP.SupplierId, MockDateTime.Object);
                Compare.SupplierWithPandO(supplier_ACT, pando_EXP);
                supplier_ACT.Orderbooks.Count().Should().Be(0);
            }
        }

        [Test]
        public void then_delete_planned_and_overdues()
        {
            int numberOfPlannedOverdues;
            var numberOfPlannedOverdues_EXP =
                fileParser.PlannedAndOverdueOrders
                .Where(p => p.SupplierId == fileParser.PlannedAndOverdueOrders.First().SupplierId)
                .Count();
            //ARRANGE
            using (var repo_SUT = new GeneratingOrderbooksRepository(new GeneratingOrderbooksContext
                                                                        (new DbContextOptionsBuilder<DbContext>()
                                                                            .UseInMemoryDatabase(databaseName: conn).Options)))
            {
                numberOfPlannedOverdues = repo_SUT.GetOrdersPerSupplierFromPANDO(fileParser.PlannedAndOverdueOrders.First().SupplierId).Count();
               //ACT
               repo_SUT.DeletePlannedAndOverdues();
            }
            //ASSERT
            using (var repo_SUT = new GeneratingOrderbooksRepository(new GeneratingOrderbooksContext
                                                            (new DbContextOptionsBuilder<DbContext>()
                                                                .UseInMemoryDatabase(databaseName: conn).Options)))
            {
                int numberOfPlannedOverdues_ACT = 
                    repo_SUT.GetOrdersPerSupplierFromPANDO(fileParser.PlannedAndOverdueOrders.First().SupplierId).Count();
                numberOfPlannedOverdues.Should().Be(numberOfPlannedOverdues_EXP);
                numberOfPlannedOverdues_ACT.Should().Be(0);                
            }
        }

        [Test]
        public void then_add_new_orderbook_to_a_supplier()
        {
            //ARRANGE
            var pando_EXP = fileParser.PlannedAndOverdueOrders.First();
            var ordersPerSupplier_EXP = fileParser.PlannedAndOverdueOrders.Where(si => si.SupplierId == pando_EXP.SupplierId).ToList();


            using (var repo_SUT = new GeneratingOrderbooksRepository(new GeneratingOrderbooksContext
                                                                        (new DbContextOptionsBuilder<DbContext>()
                                                                            .UseInMemoryDatabase(databaseName: conn).Options)))
            {
                Supplier supplier = new Supplier(pando_EXP.SupplierId, pando_EXP.SupplierName);               
                repo_SUT.SaveSupplier(supplier);
            }

            //ACT
            using (var repo_SUT = new GeneratingOrderbooksRepository(new GeneratingOrderbooksContext
                                                                        (new DbContextOptionsBuilder<DbContext>()
                                                                            .UseInMemoryDatabase(databaseName: conn).Options)))
            {
                var supplier_ACT = repo_SUT.GetSupplierWithOrderbookAndOrders(pando_EXP.SupplierId, MockDateTime.Object);
                supplier_ACT.UpdateOrderbook(ordersPerSupplier_EXP, MockDateTime.Object);
                repo_SUT.SaveSupplier(supplier_ACT);
            }
       
            //ASSERT
            using (var repo_SUT = new GeneratingOrderbooksRepository(new GeneratingOrderbooksContext
                                                            (new DbContextOptionsBuilder<DbContext>()
                                                                .UseInMemoryDatabase(databaseName: conn).Options)))
            {
                var supplier_ACT = repo_SUT.GetSupplierWithOrderbookAndOrders(pando_EXP.SupplierId, MockDateTime.Object);
                Compare.SupplierWithPandO(supplier_ACT, pando_EXP);
                supplier_ACT.Orderbooks.Count().Should().Be(1);
                Compare.OrderbookWithPandO(supplier_ACT.Orderbooks.First(), ordersPerSupplier_EXP, MockDateTime.Object);
            }
        }

        [Test]
        public void then_retrieve_orderbooks_for_suppliers_for_different_orderbook_weeks()
        {
            //ARRANGE
            var pando_EXP = fileParser.PlannedAndOverdueOrders.First();
            var ordersPerSupplier_EXP = fileParser.PlannedAndOverdueOrders.Where(si => si.SupplierId == pando_EXP.SupplierId).ToList();

            using (var repo_SUT = new GeneratingOrderbooksRepository(new GeneratingOrderbooksContext
                                                                        (new DbContextOptionsBuilder<DbContext>()
                                                                            .UseInMemoryDatabase(databaseName: conn).Options)))
            {
                Supplier supplier = new Supplier(pando_EXP.SupplierId, pando_EXP.SupplierName);
                repo_SUT.SaveSupplier(supplier);
            }

            using (var repo_SUT = new GeneratingOrderbooksRepository(new GeneratingOrderbooksContext
                                                                        (new DbContextOptionsBuilder<DbContext>()
                                                                            .UseInMemoryDatabase(databaseName: conn).Options)))
            {
                var supplier_ACT = repo_SUT.GetSupplierWithOrderbookAndOrders(pando_EXP.SupplierId, MockDateTime.Object);
                supplier_ACT.UpdateOrderbook(ordersPerSupplier_EXP, MockDateTime.Object);
                repo_SUT.SaveSupplier(supplier_ACT);
            }

            DateTime dateTime = new DateTime(2019, 8, 3);
            Mock<IDateTime> MockDateTimeSecond = new Mock<IDateTime>();
            MockDateTimeSecond.Setup(fn => fn.GetTime()).Returns(dateTime);
            PlannedAndOverdueFileParser fileParserNewDate = new PlannedAndOverdueFileParser(dateTime);
            FileReadUtility.FileRead(fileParserNewDate);
            var ordersPerSupplierSecond_EXP = fileParserNewDate.PlannedAndOverdueOrders.Where(si => si.SupplierId == pando_EXP.SupplierId).ToList();
            var pandoSecond_EXP = fileParserNewDate.PlannedAndOverdueOrders.First();
            using (var repo = new GeneratingOrderbooksRepository(new GeneratingOrderbooksContext
                                                                        (new DbContextOptionsBuilder<DbContext>()
                                                                            .UseInMemoryDatabase(databaseName: conn).Options)))
            {
                repo.DeletePlannedAndOverdues();
                repo.UpdatePlannedAndOverdues(fileParserNewDate.PlannedAndOverdueOrders);
                var supplier_ACT = repo.GetSupplierWithOrderbookAndOrders(pandoSecond_EXP.SupplierId, MockDateTimeSecond.Object);
                supplier_ACT.UpdateOrderbook(ordersPerSupplierSecond_EXP, MockDateTimeSecond.Object);
                repo.SaveSupplier(supplier_ACT);
            }

            using (var repo = new GeneratingOrderbooksRepository(new GeneratingOrderbooksContext
                                                                        (new DbContextOptionsBuilder<DbContext>()
                                                                            .UseInMemoryDatabase(databaseName: conn).Options)))
            {
                //ACT
                var firstOrderbookWeeks = repo.GetSuppliersWithOrderbookForWeek("2019:41");
                var secondOrderbookWeeks = repo.GetSuppliersWithOrderbookForWeek("2019:32");

                //ASSERT
                Compare.SupplierWithPandO(firstOrderbookWeeks.First(), pando_EXP);
                var firstOrderBook_ACT =  firstOrderbookWeeks.First().Orderbooks.First();
                firstOrderBook_ACT.SupplierId.Should().Be(pando_EXP.SupplierId);
                firstOrderBook_ACT.DateStamp.DateCreated.Should().Be(MockDateTime.Object.GetTime());
                firstOrderBook_ACT.DateStamp.DatePulled.Should().Be(MockDateTime.Object.GetTime());
                firstOrderBook_ACT.DateStamp.OrderbookWeek.Should().Be("2019:41");
                firstOrderBook_ACT.Orders.Should().BeEmpty();

                var secondOrderBook_ACT = secondOrderbookWeeks.First().Orderbooks.First();
                Compare.SupplierWithPandO(secondOrderbookWeeks.First(), pandoSecond_EXP);
                secondOrderBook_ACT.SupplierId.Should().Be(pandoSecond_EXP.SupplierId);
                secondOrderBook_ACT.DateStamp.DateCreated.Should().Be(MockDateTimeSecond.Object.GetTime());
                secondOrderBook_ACT.DateStamp.DatePulled.Should().Be(MockDateTimeSecond.Object.GetTime());
                secondOrderBook_ACT.DateStamp.OrderbookWeek.Should().Be("2019:32");
                secondOrderBook_ACT.Orders.Should().BeEmpty();
            }
        }

        [Test]
        public void then_update_existing_order_within_an_existing_orderbook_for_a_supplier()
        {  
            //ARRANGE
            var pando_EXP = fileParser.PlannedAndOverdueOrders.First();
            var ordersPerSupplier_EXP = fileParser.PlannedAndOverdueOrders.Where(si => si.SupplierId == pando_EXP.SupplierId).ToList();
            IEnumerable<PlannedAndOverdueOrder> newPlannedAndOverdueOrders;
            int numberOfOrdersInOriginalOrderBook_ACT = 0;
            using (var repo_SUT = new GeneratingOrderbooksRepository(new GeneratingOrderbooksContext
                                                                        (new DbContextOptionsBuilder<DbContext>()
                                                                            .UseInMemoryDatabase(databaseName: conn).Options)))
            {
                Supplier supplier = new Supplier(pando_EXP.SupplierId, pando_EXP.SupplierName);
                supplier.UpdateOrderbook(ordersPerSupplier_EXP, MockDateTime.Object);
                repo_SUT.SaveSupplier(supplier);
            }

            using (var repo_SUT = new GeneratingOrderbooksRepository(new GeneratingOrderbooksContext
                                                                        (new DbContextOptionsBuilder<DbContext>()
                                                                            .UseInMemoryDatabase(databaseName: conn).Options)))
            {
                //create new order
                newPlannedAndOverdueOrders = new List<PlannedAndOverdueOrder>()
                {
                    new PlannedAndOverdueOrder(
                   pando_EXP.Id, pando_EXP.SupplierId, pando_EXP.SupplierName, "part number", "part description",
                  "purchase order",2, 2, 2, MockDateTime.Object.GetTime(),
                   MockDateTime.Object.GetTime(),MockDateTime.Object.GetTime())
                };
                var supplier_ACT = repo_SUT.GetSupplierWithOrderbookAndOrders(pando_EXP.SupplierId, MockDateTime.Object);
                numberOfOrdersInOriginalOrderBook_ACT = supplier_ACT.Orderbooks.First().Orders.Count();
                supplier_ACT.UpdateOrderbook(newPlannedAndOverdueOrders, MockDateTime.Object);
                //ACT
                repo_SUT.SaveSupplier(supplier_ACT);
            }
            //ASSERT
            using (var repo_SUT = new GeneratingOrderbooksRepository(new GeneratingOrderbooksContext
                                                                        (new DbContextOptionsBuilder<DbContext>()
                                                                            .UseInMemoryDatabase(databaseName: conn).Options)))
            {
                var supplier_ACT = repo_SUT.GetSupplierWithOrderbookAndOrders(pando_EXP.SupplierId, MockDateTime.Object);
                numberOfOrdersInOriginalOrderBook_ACT.Should().Be(ordersPerSupplier_EXP.Count());
                supplier_ACT.Orderbooks.First().Orders.Count().Should().Be(newPlannedAndOverdueOrders.Count());
                Compare.SupplierWithPandO(supplier_ACT, pando_EXP);
                Compare.OrderbookWithPandO(supplier_ACT.Orderbooks.First(), newPlannedAndOverdueOrders, MockDateTime.Object);
            }
        }

        [Test]
        public void then_add_new_order_to_an_existing_orderbook_for_a_supplier()
        {
            //ARRANGE
            var pando_EXP = fileParser.PlannedAndOverdueOrders.First();
            var ordersPerSupplier_EXP = fileParser.PlannedAndOverdueOrders
                .Where(si => si.SupplierId == pando_EXP.SupplierId).ToList();
            IEnumerable<PlannedAndOverdueOrder> newPlannedAndOverdueOrders;
  
            int numberOfOrdersInOriginalOrderBook_ACT = 0;
            //register supplier with an order book aswell
            using (var repo_SUT = new GeneratingOrderbooksRepository(new GeneratingOrderbooksContext
                                                                        (new DbContextOptionsBuilder<DbContext>()
                                                                            .UseInMemoryDatabase(databaseName: conn).Options)))
            {
                Supplier supplier = new Supplier(pando_EXP.SupplierId, pando_EXP.SupplierName);
                supplier.UpdateOrderbook(ordersPerSupplier_EXP, MockDateTime.Object);
                repo_SUT.SaveSupplier(supplier);
            }

            using (var repo_SUT = new GeneratingOrderbooksRepository(new GeneratingOrderbooksContext
                                                                        (new DbContextOptionsBuilder<DbContext>()
                                                                            .UseInMemoryDatabase(databaseName: conn).Options)))
            {
                var supplier_ACT = repo_SUT.GetSupplierWithOrderbookAndOrders(pando_EXP.SupplierId, MockDateTime.Object);
                numberOfOrdersInOriginalOrderBook_ACT = supplier_ACT.Orderbooks.First().Orders.Count();
                newPlannedAndOverdueOrders = new List<PlannedAndOverdueOrder>()
                {
                    new PlannedAndOverdueOrder(
                   "uniquenumber", pando_EXP.SupplierId, pando_EXP.SupplierName, pando_EXP.PartNumber, pando_EXP.PartDescription,
                   pando_EXP.PurchaseOrder, pando_EXP.POLineItem, pando_EXP.POSchedLine, pando_EXP.OpenPOQty, pando_EXP.ItemDeliveryDate,
                   pando_EXP.StatDeliverySchedule, pando_EXP.DatePulled)
                };
                supplier_ACT.UpdateOrderbook(newPlannedAndOverdueOrders, MockDateTime.Object);

                //ACT
                repo_SUT.SaveSupplier(supplier_ACT);
            }
            //ASSERT
            using (var repo_SUT = new GeneratingOrderbooksRepository(new GeneratingOrderbooksContext
                                                                        (new DbContextOptionsBuilder<DbContext>()
                                                                            .UseInMemoryDatabase(databaseName: conn).Options)))
            {
                var supplier_ACT = repo_SUT.GetSupplierWithOrderbooks(pando_EXP.SupplierId);

                Compare.SupplierWithPandO(supplier_ACT, pando_EXP);
                supplier_ACT.Orderbooks.Count().Should().Be(1);
                numberOfOrdersInOriginalOrderBook_ACT.Should().Be(ordersPerSupplier_EXP.Count());
                //check that the original order is gone
                supplier_ACT.Orderbooks.First().Orders.Any(o => o.PandOId == pando_EXP.Id).Should().BeFalse();
                Compare.OrderbookWithPandO(supplier_ACT.Orderbooks.First(), newPlannedAndOverdueOrders, MockDateTime.Object);
            }
        }

        [Test]
        public void then_get_supplier_code_from_planned_and_overdues()
        {
            //ARRANGE            
            var supplierCodes_EXP = fileParser.PlannedAndOverdueOrders
                .Select(sc => sc.SupplierId).Distinct().ToList();
            using (var repo_SUT = new GeneratingOrderbooksRepository(new GeneratingOrderbooksContext
                                                                        (new DbContextOptionsBuilder<DbContext>()
                                                                            .UseInMemoryDatabase(databaseName: conn).Options)))
            {
                //ACT
                var supplierCodes_ACT = repo_SUT.GetDistinctSupplierCodesFromPANDO();
                //ASSERT
                Compare.Collection(supplierCodes_ACT,
                                   supplierCodes_EXP);
            }
        }

        [Test]
        public void validate_planned_and_overdues()
        {
            //ARRANGE        
           using (var repo_SUT = new GeneratingOrderbooksRepository(new GeneratingOrderbooksContext
                                                                        (new DbContextOptionsBuilder<DbContext>()
                                                                            .UseInMemoryDatabase(databaseName: conn).Options)))
            {
                //ACT
                Action act = () => repo_SUT.ValidatePlannedAndOverDues();
                //ASSERT
                act.Should().NotThrow<DomainException>();
            }
        }

        [Test]
        public void then_validate_planned_and_overdues_and_throw_exception_if_date_pulled_not_consistent_in_planned_and_overdues()
        {
            //ARRANGE        
            PlannedAndOverdueFileParser invalidFileParser;
            DateTime dateTime = DateTime.Now;
            invalidFileParser = new PlannedAndOverdueFileParser(dateTime);
            FileReadUtility.FileRead(invalidFileParser, "MIS PLY Purchasing Data-inconsistent_date_pulled.csv");
            using (var repo_SUT = new GeneratingOrderbooksRepository(new GeneratingOrderbooksContext
                                                                        (new DbContextOptionsBuilder<DbContext>()
                                                                            .UseInMemoryDatabase(databaseName: conn).Options)))
            {

                repo_SUT.UpdatePlannedAndOverdues(invalidFileParser.PlannedAndOverdueOrders);
                //ACT
                Action act = () => repo_SUT.ValidatePlannedAndOverDues();
                //ASSERT
                act.Should().Throw<DomainException>();
            }
        }

        [Test]
        public void then_get_orders_per_supplier_from_planned_and_overdues()
        {
            //ARRANGE     
            var supplierId_EXP = fileParser.PlannedAndOverdueOrders.First().SupplierId;
            var ordersPerSupplier_EXP = fileParser.PlannedAndOverdueOrders.Where(si => si.SupplierId == supplierId_EXP).ToList();
            using (var repo_SUT = new GeneratingOrderbooksRepository(new GeneratingOrderbooksContext
                                                                        (new DbContextOptionsBuilder<DbContext>()
                                                                            .UseInMemoryDatabase(databaseName: conn).Options)))
            {
                //ACT
                var ordersPerSupplier_ACT = repo_SUT.GetOrdersPerSupplierFromPANDO(supplierId_EXP);
                //ASSERT
                Compare.Collection(ordersPerSupplier_ACT,
                                   ordersPerSupplier_EXP);
                ordersPerSupplier_ACT.Select(si => si.SupplierId).Distinct().Count().Should().Be(1);

            }
        }

        [Test]
        public void then_get_supplier_name_from_planned_and_overdues()
        {
            //ARRANGE            
            using (var repo_SUT = new GeneratingOrderbooksRepository(new GeneratingOrderbooksContext
                                                                      (new DbContextOptionsBuilder<DbContext>()
                                                                          .UseInMemoryDatabase(databaseName: conn).Options)))
            {
                //ACT
                var supplierName_ACT = repo_SUT.GetSupplierNameFromPANDO(fileParser.PlannedAndOverdueOrders.First().SupplierId);
                //ASSERT
                supplierName_ACT.Should().Be(fileParser.PlannedAndOverdueOrders.First().SupplierName);
            }
        }

        [Test]
        public void then_throw_domain_exception_when_getting_supplier_name_from_planned_and_overdue_because_supplier_does_not_exist()
        {
            //ARRANGE
            using (var repo_SUT = new GeneratingOrderbooksRepository(new GeneratingOrderbooksContext
                                                                      (new DbContextOptionsBuilder<DbContext>()
                                                                          .UseInMemoryDatabase(databaseName: conn).Options)))
            {
                //ACT
                Action actionSut = () => repo_SUT.GetSupplierNameFromPANDO(5);
                //ASSERT
                actionSut.Should().Throw<Exception>();
            }
        }

        [Test]
        public void then_attempt_to_retreive_supplier_with_orderbooks_for_unregistered_supplier()
        {
            //ARRANGE       
            var supplierId_EXP = fileParser.PlannedAndOverdueOrders.First().SupplierId;
            var ordersPerSupplier_EXP = fileParser.PlannedAndOverdueOrders.Where(si => si.SupplierId == supplierId_EXP).ToList();
            using (var repo_SUT = new GeneratingOrderbooksRepository(new GeneratingOrderbooksContext
                                                                        (new DbContextOptionsBuilder<DbContext>()
                                                                            .UseInMemoryDatabase(databaseName: conn).Options)))
            {
                //ACT
                var supplier_ACT = repo_SUT.GetSupplierWithOrderbooks(4);
                //ASSERT
                supplier_ACT.Should().BeNull();
            }
        }

        [Test]
        public void then_attempt_to_retrieve_supplier_with_orderbooks_for_registered_supplier()
        {
            //ARRANGE    
            //Seed an orderbook
            // TODO UP TO HERE #1 need to seed order book table which cant be done without supplier
            using (var repo_SUT = new GeneratingOrderbooksRepository(new GeneratingOrderbooksContext
                                                                        (new DbContextOptionsBuilder<DbContext>()
                                                                            .UseInMemoryDatabase(databaseName: conn).Options)))
            {
                //ACT
                var supplier_ACT = repo_SUT.GetSupplierWithOrderbooks(4);
                //ASSERT
                supplier_ACT.Should().BeNull();
            }
        }
    }
}
