using DMT.SharedKernel.Interface;
using DMT.GeneratingOrderbooks.EventService;
using DMT.Web.EventService;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System.Web.Mvc;
using System;

namespace GeneratingOrderbooks.given_an_orderbook.Presentation.Test
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
        public void then_create_new_orderbooks()
        {
            //ARRANGE
            //TODO: Mock service layer to create a new orderbooks
            MockGeneratingOrderbooksService.Setup(fn => 
                 fn.GenerateOrderbooks(It.IsAny<IDateTime>()));

            //ACT
            //TODO: Call the action in the GenerateOrderbooksController
            var actualActionResult = GenerateOrderbookController_SUT.GenerateOrderbooks();

            //ASSERT
            //TODO: Verify the correct method is called
            MockGeneratingOrderbooksService.Verify(fn => 
                fn.GenerateOrderbooks(It.IsAny<IDateTime>()), Times.Once);
            actualActionResult.Should().BeOfType<EmptyResult>();
        }

        [Test]
        public void then_handle_orderbook_saved_event()
        {
            //ARRANGE
            OrderBookSavedHandler orderBookSavedHandler_SUT = new OrderBookSavedHandler();

            //ACT
            Action act = () => orderBookSavedHandler_SUT.Handle(It.IsAny<OrderbookSavedEvent>());
   
            //ASSERT 
            act.Should().NotThrow<Exception>();
        }
    }
}