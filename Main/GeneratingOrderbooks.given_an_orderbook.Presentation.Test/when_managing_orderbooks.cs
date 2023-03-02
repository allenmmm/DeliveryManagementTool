using DMT.DTO;
using DMT.Web.ViewModels;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SharedKernel.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GeneratingOrderbooks.given_an_orderbook.Presentation.Test
{
    [TestFixture]
    public class when_managing_orderbooks : TestBase
    {
        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
        }

        [Test]
        public void then_view_all_orderbook_weeks()
        {
            //ARRANGE
            //TODO: Create list of expected orderbook weeks
            //      Mock service layer to return list of expected orderbook weeks
            IReadOnlyList<OrderbookWeekDTO> expectedOrderbookWeeks = new List<OrderbookWeekDTO>
            {
                new OrderbookWeekDTO("2019: 01"),
                new OrderbookWeekDTO("2019: 02"),
                new OrderbookWeekDTO("2019: 03")
            };
            MockGeneratingOrderbooksService.Setup(fn => fn.GetOrderbookWeeks())
                                           .Returns(expectedOrderbookWeeks);

            //ACT
            //TODO: Call the view code in GenerateOrderbooksController
            var actualActionResult = GenerateOrderbookController_SUT.Index();

            //ASSERT
            //TODO: Verify the correct method is called
            //      Verify the correct types are returned
            actualActionResult.Should().BeOfType<ViewResult>();

            var actualView = (ViewResult)actualActionResult;
            actualView.Model.Should().BeOfType<GenerateOrderbooksIndexVM>();

            var generateOrderbooksIndexVM = actualView.Model as GenerateOrderbooksIndexVM;
            var actualOrderbookWeeks = generateOrderbooksIndexVM.OrderbookWeeks;
            actualOrderbookWeeks.Should().BeOfType<List<OrderbookWeekDTO>>();
            actualOrderbookWeeks.Count().Should().Be(expectedOrderbookWeeks.Count);

            Compare.Collection(actualOrderbookWeeks, expectedOrderbookWeeks);

            MockGeneratingOrderbooksService.Verify(fn => fn.GetOrderbookWeeks(), Times.Once());
        }

        [Test]
        public void then_view_all_orderbook_previews_without_date()
        {
            //ARRANGE
            //TODO: Create list of expected orderbook previews
            //      Mock service layer to return list of expected orderbook previews
            var dateTime = DateTime.Now;

            IReadOnlyList<OrderbookPreviewDTO> orderbookPreviews = new List<OrderbookPreviewDTO>
            {
                new OrderbookPreviewDTO(1, dateTime, 123, "description"),
                new OrderbookPreviewDTO(2, dateTime, 456, "description"),
                new OrderbookPreviewDTO(3, dateTime, 789, "description")
            };
            OrderbookPreviewsDTO expectedOrderbookPreviews = 
                new OrderbookPreviewsDTO(dateTime, orderbookPreviews);
            MockGeneratingOrderbooksService.Setup(fn => fn.GetOrderbookPreviews(""))
                                           .Returns(expectedOrderbookPreviews);

            //ACT
            //TODO: Call the view code in GenerateOrderbooksController
            var actualActionResult = GenerateOrderbookController_SUT.Index();
      
            //ASSERT
            //TODO: Verify the correct method is called
            //      Verify the correct types are returned
            actualActionResult.Should().BeOfType<ViewResult>();
      
            var actualView = (ViewResult)actualActionResult;
            actualView.Model.Should().BeOfType<GenerateOrderbooksIndexVM>();

            var generateOrderbooksIndexVM = actualView.Model as GenerateOrderbooksIndexVM;
            generateOrderbooksIndexVM.OrderbookPreviews.OrderbookWeekStart.Should().Be(dateTime);

            var actualOrderbookPreviews = generateOrderbooksIndexVM.OrderbookPreviews.OrderbookPreviewDTOs;
            actualOrderbookPreviews.Should().BeOfType<List<OrderbookPreviewDTO>>();
            actualOrderbookPreviews.Count.Should().Be(expectedOrderbookPreviews.OrderbookPreviewDTOs.Count);

            Compare.Collection(actualOrderbookPreviews, expectedOrderbookPreviews.OrderbookPreviewDTOs);
            
            MockGeneratingOrderbooksService.Verify(fn => fn.GetOrderbookPreviews(""), Times.Once());
        }

        [Test]
        public void then_view_all_orderbook_previews_with_date()
        {
            //ARRANGE
            //TODO: Create list of expected orderbook previews
            //      Mock service layer to return list of expected orderbook previews
            var orderbookWeek_ACT = "2019:02";
            var dateTime = DateTime.Now;

            IReadOnlyList<OrderbookPreviewDTO> orderbookPreviews = new List<OrderbookPreviewDTO>
            {
                new OrderbookPreviewDTO(1, dateTime, 123, "description"),
                new OrderbookPreviewDTO(2, dateTime, 456, "description"),
                new OrderbookPreviewDTO(3, dateTime, 789, "description")
            };
            OrderbookPreviewsDTO expectedOrderbookPreviews =
                new OrderbookPreviewsDTO(dateTime, orderbookPreviews);
            MockGeneratingOrderbooksService.Setup(fn => fn.GetOrderbookPreviews(It.IsAny<string>()))
                                           .Callback<string>((orderbookWeek) => { orderbookWeek_ACT = orderbookWeek; })
                                           .Returns(expectedOrderbookPreviews);
      
            //ACT
            //TODO: Call the view code in GenerateOrderbooksController
            var actualResult = GenerateOrderbookController_SUT._OrderbookPreview("2019:01");

            //ASSERT
            //TODO: Verify the correct method is called
            //      Verify the correct types are returned
            orderbookWeek_ACT.Should().Be("2019:01");

            actualResult.Should().BeOfType<PartialViewResult>();
      
            var actualView = (PartialViewResult)actualResult;
            actualView.Model.Should().BeOfType<OrderbookPreviewsDTO>();

            var orderbookPreviewsDTO = actualView.Model as OrderbookPreviewsDTO;
            orderbookPreviewsDTO.OrderbookWeekStart.Should().Be(dateTime);

            var actualOrderbookPreviews = orderbookPreviewsDTO.OrderbookPreviewDTOs;
            actualOrderbookPreviews.Should().BeOfType<List<OrderbookPreviewDTO>>();
            actualOrderbookPreviews.Count.Should().Be(expectedOrderbookPreviews.OrderbookPreviewDTOs.Count);
      
            Compare.Collection(actualOrderbookPreviews, expectedOrderbookPreviews.OrderbookPreviewDTOs);
      
            MockGeneratingOrderbooksService.Verify(fn => fn.GetOrderbookPreviews(It.IsAny<string>()), Times.Once());
        }
    }
}