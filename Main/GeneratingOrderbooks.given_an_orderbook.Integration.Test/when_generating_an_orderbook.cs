using DMT.DTO;
using DMT.GeneratingOrderbooks.Service;
using DMT.GeneratingOrderBooks.Data;
using DMT.ManagingNotifications.Data;
using DMT.ManagingNotifications.Service;
using DMT.SharedKernel;
using DMT.SharedKernel.Interface;
using DMT.SharedKernel.ValueObjects;
using DMT.Web.Controllers;
using FluentAssertions;
using FluentAssertions.Extensions;
using Moq;
using MvcContrib.TestHelper;
using NUnit.Framework;
using SharedKernel.Test;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneratingOrderbooks.given_an_orderbook.Integration.Test
{
    [TestFixture]
    public class when_generating_an_orderbook : TestBase
    {
        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
        }

        [Test]
        public void then_generate_orderbooks_for_different_orderbook_weeks()
        {
            //ARRANGE
            //test creates. 2 orderbooks in different orderbook weeks, then
            //attempts to create a third in the same orderbook week as the second
            //and ensure it is overwritten
            Mock<IDateTime> firstDateTime = new Mock<IDateTime>();
            firstDateTime.Setup(fn => fn.GetTime()).Returns(new DateTime(2019, 3, 8));

            Mock<IDateTime> secondDateTime = new Mock<IDateTime>();
            secondDateTime.Setup(fn => fn.GetTime()).Returns(new DateTime(2019, 6, 9,11,11,11));

            Mock<IDateTime> thirdDateTime = new Mock<IDateTime>();
            thirdDateTime.Setup(fn => fn.GetTime()).Returns(new DateTime(2019, 6, 9,12,12,12));

            PlannedAndOverdueFileParser fileParser = 
                new PlannedAndOverdueFileParser(firstDateTime.Object.GetTime());
                                                        FileReadUtility.FileRead(fileParser);

            var firstPando_EXP = fileParser.PlannedAndOverdueOrders.First();
            var firstPandos_EXP = fileParser.PlannedAndOverdueOrders.
                            Where(s => s.Id == firstPando_EXP.Id).ToList();


            PopulateTable("PlannedAndOverdueOrders",
                           SqlSeeder.SetPandOSQLValues(firstPandos_EXP.GetRange(0, 1)));

            PopulateTable("Supplier",
                           SqlSeeder.SetSupplierSQLValues(firstPandos_EXP.GetRange(0, 1)));

            //write first orderbook
            using (var repo_SUT = new GeneratingOrderbooksRepository(GeneratingOrderbooksContext))
            {
                GeneratingOrderbooksService generatingOrderbooksService =
                    new GeneratingOrderbooksService(repo_SUT);

                generatingOrderbooksService.GenerateOrderbooks(firstDateTime.Object);
            }

            fileParser = new PlannedAndOverdueFileParser(secondDateTime.Object.GetTime());
                            FileReadUtility.FileRead(fileParser);

            firstPando_EXP = fileParser.PlannedAndOverdueOrders.First();
            var secondPandos_EXP = fileParser.PlannedAndOverdueOrders.
                            Where(s => s.Id == firstPando_EXP.Id).ToList();

            PopulateTable("PlannedAndOverdueOrders",
                           SqlSeeder.SetPandOSQLValues(secondPandos_EXP.GetRange(0, 1)));

            //write second orderbook
            using (var repo_SUT = new GeneratingOrderbooksRepository(GeneratingOrderbooksContext))
            {
                GeneratingOrderbooksService generatingOrderbooksService 
                    = new GeneratingOrderbooksService(repo_SUT);

                generatingOrderbooksService.GenerateOrderbooks(secondDateTime.Object);
            }

            //third orderbook but this time for the same orderbook week
            fileParser =
                    new PlannedAndOverdueFileParser(thirdDateTime.Object.GetTime());
                         FileReadUtility.FileRead(fileParser);

            firstPando_EXP = fileParser.PlannedAndOverdueOrders.First();
            var thirdPandos_EXP = fileParser.PlannedAndOverdueOrders.
                            Where(s => s.Id == firstPando_EXP.Id).ToList();

            PopulateTable("PlannedAndOverdueOrders",
                           SqlSeeder.SetPandOSQLValues(thirdPandos_EXP.GetRange(0, 1)));
            using (var repo_SUT = new GeneratingOrderbooksRepository(GeneratingOrderbooksContext))
            {
                GeneratingOrderbooksService generatingOrderbooksService
                    = new GeneratingOrderbooksService(repo_SUT);
                //ACT
                generatingOrderbooksService.GenerateOrderbooks(thirdDateTime.Object);
            }

            //ASSERT
            using (var repo_SUT = new GeneratingOrderbooksRepository(GeneratingOrderbooksContext))
            {
                var orderbooks_ACT = repo_SUT.GetSupplierWithOrderbooks(firstPando_EXP.SupplierId);
                orderbooks_ACT.Orderbooks.Count().Should().Be(2);

                var firstOrderbook = repo_SUT.GetSupplierWithOrderbookAndOrders(firstPando_EXP.SupplierId,
                                                                                firstDateTime.Object);
                Compare.OrderbookWithPandO(firstOrderbook.Orderbooks.First(), 
                                            firstPandos_EXP, firstDateTime.Object);
                var thirdOrderbook = repo_SUT.GetSupplierWithOrderbookAndOrders(firstPando_EXP.SupplierId,
                                                                thirdDateTime.Object);
                Compare.OrderbookWithPandO(thirdOrderbook.Orderbooks.First(),
                                            thirdPandos_EXP, thirdDateTime.Object);
            }
        }

        [Test]
        public void then_create_orderbooks_from_valid_pando()
        {
            //ARRANGE
            DateTime dateTime = DateTime.Now;
            var seedTime = dateTime.ToString("yyyyMMddHHmmss");
            var datePulled = DateTime.ParseExact
                (seedTime, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
            Seed(datePulled);

            var obw = OrderbookWeek_VO.Create(dateTime).FormatString();
            List<NotificationDTO> notifications_EXP = new List<NotificationDTO>()
            {
                new NotificationDTO(dateTime,
                                    "Orderbook(s) generation started (Week "+obw+")",
                                    NotificationState.Information),
                new NotificationDTO(dateTime,
                                        "All expected orderbook(s) generated (8/8 succeeded for Week "+obw+")",
                                        NotificationState.Success)
            };

            using (var repo_SUT = new GeneratingOrderbooksRepository(GeneratingOrderbooksContext))
            {
                Action firstAction = () => repo_SUT.GetDistinctSupplierCodesFromPANDO();
                firstAction.Should().NotThrow<DomainException>();

                GeneratingOrderbooksService GeneratingOrderbooksService = new GeneratingOrderbooksService(repo_SUT);
                TestControllerBuilder testControllerBuilder = new TestControllerBuilder();
                GenerateOrderbooksController HomeController = testControllerBuilder.
                    CreateController<GenerateOrderbooksController>(GeneratingOrderbooksService);
                //ACT
                Action act = () => HomeController.GenerateOrderbooks();
                act.Should().NotThrow<DomainException>();
            }

            //ASSERT - what was wrote to the database is equivalent to the pando
            using (var repo_SUT = new GeneratingOrderbooksRepository(GeneratingOrderbooksContext))
            {

                PlannedAndOverdueFileParser FileParser = new PlannedAndOverdueFileParser(datePulled);
                FileReadUtility.FileRead(FileParser);
                var suppliers_EXP = FileParser.PlannedAndOverdueOrders.Select(po => po.SupplierId).Distinct().ToList();

                //GET EXPECTED SUPPLIER ID
                foreach (var supplierId_EXP in suppliers_EXP)
                {   //GET EXPECTED ORDERS PER EXPECTED SUPPLIER
                    var orders_EXP = FileParser.PlannedAndOverdueOrders.Where(po => po.SupplierId == supplierId_EXP).ToList();
                    //  var supplier_ACT = GeneratingOrderbooksRepo.GetSupplierWithOrderbooks(supplierId_EXP);
                    Mock<IDateTime> mockDateTimeConfig = new Mock<IDateTime>();
                    mockDateTimeConfig.Setup(fn => fn.GetTime()).Returns(dateTime);
                    var orderbooks_ACT = repo_SUT.GetSupplierWithOrderbookAndOrders(supplierId_EXP, mockDateTimeConfig.Object);
                    Compare.SupplierWithPandO(orderbooks_ACT, orders_EXP.First());
                    Compare.OrderbookWithPandO(orderbooks_ACT.Orderbooks.First(), orders_EXP, mockDateTimeConfig.Object);

                };
                Action thirdAction = () => repo_SUT.GetDistinctSupplierCodesFromPANDO();
                thirdAction.Should().Throw<DomainException>();
            }

            // check that notification was raised
            using (var repo_SUT = new ManagingNotificationsRepository(ManagingNotificationsContext))
            {
                ManagingNotificationsService managingNotificationsService = new ManagingNotificationsService(repo_SUT);
                var notifications_ACT = managingNotificationsService.GetNotifications();

                foreach (var notification_EXP in notifications_EXP)
                {
                    var notification_ACT =
                    notifications_ACT.Where(n => n.Description == notification_EXP.Description).First();
                    notification_ACT.DateRaised.Should().BeWithin(20.Seconds()).After(notification_EXP.DateRaised);
                    notification_ACT.Description.Should().Be(notification_EXP.Description);
                    notification_ACT.NotificationState.Should().Be(notification_EXP.NotificationState);
                }
                notifications_ACT.Count().Should().Be(10);
                var x = notifications_ACT.Where(n => n.Description.Contains("Orderbook generated"))
                     .Count().Should().Be(8);         
            }
        }

        [Test]
        public void then_terminate_orderbook_generation_on_db_error()
        {
            //ARRANGE 
            DateTime dateTime = DateTime.Now;
            var obw = OrderbookWeek_VO.Create(dateTime).FormatString();
            List<NotificationDTO> notifications_EXP = new List<NotificationDTO>()
            {
                new NotificationDTO(dateTime,
                                    "Orderbook(s) generation started (Week "+obw+")",
                                    NotificationState.Information),
                new NotificationDTO(dateTime,
                                   "Orderbook(s) generation terminated.  " +
                                   "Planned and overdues table empty or could not be validated/accessed " +
                                   "(Object reference not set to an instance of an object.)",
                                   NotificationState.Error),
                new NotificationDTO(dateTime,
                                   "The planned and overdues table could not be deleted,this will potentially" +
                                   " corrupt future orderbook generations (Object reference not set to an instance of an object.)",
                                   NotificationState.Warning),
                new NotificationDTO(dateTime,
                                   "Errors occurred during generating of orderbook(s) (Week "+obw+")",
                                   NotificationState.Error)
            };

            using (var repo_SUT = new GeneratingOrderbooksRepository(null))
            {
                GeneratingOrderbooksService GeneratingOrderbooksService = new GeneratingOrderbooksService(repo_SUT);
                TestControllerBuilder testControllerBuilder = new TestControllerBuilder();
                GenerateOrderbooksController HomeController = 
                    testControllerBuilder.CreateController<GenerateOrderbooksController>(GeneratingOrderbooksService);

                //ACT
                Action act = () => HomeController.GenerateOrderbooks();
                act.Should().NotThrow<DomainException>();
            }
            using (var repo_SUT = new ManagingNotificationsRepository(ManagingNotificationsContext))
            {
                ManagingNotificationsService managingNotificationsService = new ManagingNotificationsService(repo_SUT);
                var notifications_ACT = managingNotificationsService.GetNotifications();
                notifications_ACT.Count().Should().Be(notifications_EXP.Count);
                int i = 0;
                foreach (var notification_ACT in notifications_ACT)
                {
                    var notification_EXP = notifications_EXP.ElementAt(i++);
                    notification_ACT.DateRaised.Should().BeWithin(10.Seconds()).After(notification_EXP.DateRaised);
                    notification_ACT.Description.Should().Be(notification_EXP.Description);
                    notification_ACT.NotificationState.Should().Be(notification_EXP.NotificationState);
                }
            }
        }

        [Test]
        public void then_terminate_orderbook_generation_when_pando_table_content_invalid()
        {
            //ARRANGE
            DateTime dateTime = DateTime.Now;
            var obw = OrderbookWeek_VO.Create(dateTime).FormatString();
            List<NotificationDTO> notifications_EXP = new List<NotificationDTO>()
            {
                new NotificationDTO(dateTime,
                                    "Orderbook(s) generation started (Week "+obw+")",
                                    NotificationState.Information),
                new NotificationDTO(dateTime,
                                   "Orderbook(s) generation terminated.  Planned and overdues table empty or " +
                                   "could not be validated/accessed (Domain Exception - Date pulled not consistent " +
                                   "throughout planned and overdues)",
                                   NotificationState.Error),
                new NotificationDTO(dateTime,
                                   "Errors occurred during generating of orderbook(s) (Week "+obw+")",
                                   NotificationState.Error)
            };

            PlannedAndOverdueFileParser FileParser = new PlannedAndOverdueFileParser(DateTime.MinValue);
            FileReadUtility.FileRead(FileParser, "MIS PLY Purchasing Data-DatePulledInconsistent.csv");
            using (var repo = new GeneratingOrderbooksRepository(GeneratingOrderbooksContext))
            {
                repo.UpdatePlannedAndOverdues(FileParser.PlannedAndOverdueOrders);
            }

            //ACT
            using (var repo_SUT = new GeneratingOrderbooksRepository(GeneratingOrderbooksContext))
            {
                GeneratingOrderbooksService GeneratingOrderbooksService = new GeneratingOrderbooksService(repo_SUT);
                TestControllerBuilder testControllerBuilder = new TestControllerBuilder();
                GenerateOrderbooksController HomeController = 
                    testControllerBuilder.CreateController<GenerateOrderbooksController>(GeneratingOrderbooksService);

                //ASSERT
                Action act = () => HomeController.GenerateOrderbooks();
                act.Should().NotThrow<DomainException>();
            }

            using (var repo_SUT = new ManagingNotificationsRepository(ManagingNotificationsContext))
            {
                ManagingNotificationsService managingNotificationsService = new ManagingNotificationsService(repo_SUT);
                var notifications_ACT = managingNotificationsService.GetNotifications();
                notifications_ACT.Count().Should().Be(notifications_EXP.Count);
                int i = 0;
                foreach (var notification_ACT in notifications_ACT)
                {
                    var notification_EXP = notifications_EXP.ElementAt(i++);
                    notification_ACT.DateRaised.Should().BeWithin(30.Seconds()).After(notification_EXP.DateRaised);
                    notification_ACT.Description.Should().Be(notification_EXP.Description);
                    notification_ACT.NotificationState.Should().Be(notification_EXP.NotificationState);
                }
            }
        }
    }
}
