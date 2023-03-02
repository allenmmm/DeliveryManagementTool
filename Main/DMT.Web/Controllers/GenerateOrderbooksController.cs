using DMT.DTO;
using DMT.GeneratingOrderbooks.Service.Interfaces;
using DMT.SharedKernel;
using DMT.Web.ViewModels;
using System.Web.Mvc;

namespace DMT.Web.Controllers
{
    public class GenerateOrderbooksController : Controller
    {
        private readonly IGeneratingOrderbooksService _GeneratingOrderbookService;

        public GenerateOrderbooksController(IGeneratingOrderbooksService generatingOrderbooksService)
        {
            _GeneratingOrderbookService = generatingOrderbooksService;
        }

        public ActionResult Index()
        {
            var generateOrderbooksIndexVM = 
                new GenerateOrderbooksIndexVM(_GeneratingOrderbookService.GetOrderbookWeeks(),
                                              _GeneratingOrderbookService.GetOrderbookPreviews());
            return View(generateOrderbooksIndexVM);
        }

        public OrderbookDTO GetOrderbook(ulong id)
        {
            return (_GeneratingOrderbookService.GetOrderbook(id));
        }

        public ActionResult _OrderbookPreview(string orderbookWeek)
        {
            OrderbookPreviewsDTO orderbookPreviews =
                _GeneratingOrderbookService.GetOrderbookPreviews(orderbookWeek);
            return PartialView("_OrderbookPreview", orderbookPreviews);
        }

        public EmptyResult GenerateOrderbooks()
        {
            _GeneratingOrderbookService.GenerateOrderbooks(new DateTimeConfig());
            return new EmptyResult();
        }
    }
}