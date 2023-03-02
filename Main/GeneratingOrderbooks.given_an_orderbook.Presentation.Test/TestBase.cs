using DMT.GeneratingOrderbooks.Service.Interfaces;
using DMT.Web.Controllers;
using Moq;
using MvcContrib.TestHelper;

namespace GeneratingOrderbooks.given_an_orderbook.Presentation.Test
{
    public abstract class TestBase
    {
        protected Mock<IGeneratingOrderbooksService> MockGeneratingOrderbooksService;
        protected GenerateOrderbooksController GenerateOrderbookController_SUT;

        protected virtual void SetUp()
        {
            MockGeneratingOrderbooksService = new Mock<IGeneratingOrderbooksService>();
            TestControllerBuilder testControllerBuilder = new TestControllerBuilder();
            GenerateOrderbookController_SUT = 
                testControllerBuilder.CreateController<GenerateOrderbooksController>(MockGeneratingOrderbooksService.Object);
        }
    }
}