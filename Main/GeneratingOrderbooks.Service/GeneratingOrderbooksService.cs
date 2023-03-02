using DMT.GeneratingOrderBooks.Domain.Entities;
using DMT.SharedKernel;
using DMT.GeneratingOrderbooks.Service.Interfaces;
using DMT.GeneratingOrderBooks.Domain.Interfaces;
using System;
using DMT.DTO;
using DMT.ManagingNotifications.EventService;
using System.Collections.Generic;
using System.Linq;
using DMT.SharedKernel.ValueObjects;
using DMT.SharedKernel.Interface;

namespace DMT.GeneratingOrderbooks.Service
{
    public class GeneratingOrderbooksService : IGeneratingOrderbooksService
    {
        private readonly IGeneratingOrderbooksRepo _GeneratingOrderbooksRepo;

        public GeneratingOrderbooksService(IGeneratingOrderbooksRepo generatingOrderbooksRepo)
        {
            _GeneratingOrderbooksRepo = generatingOrderbooksRepo;
        }

        public void GenerateOrderbooks(IDateTime dateTime)
        {
            Guid overallOrderBookStatus = NotificationCodes.OrderbooksGeneratedOK;
            IEnumerable<int> supplierCodes = null;
            int numberOfOrderbooksGenerated = 0;
            try
            {
                RaiseNotification(NotificationCodes.OrderbookGenerationStarted);

                _GeneratingOrderbooksRepo.ValidatePlannedAndOverDues();
                supplierCodes = _GeneratingOrderbooksRepo.GetDistinctSupplierCodesFromPANDO();
                foreach (var supplierCode in supplierCodes)
                {
                    Supplier supplier = null;
                    try
                    {
                        supplier = _GeneratingOrderbooksRepo.GetSupplierWithOrderbookAndOrders(supplierCode, dateTime);
                        if (supplier == null)
                        {
                            var supplierName = _GeneratingOrderbooksRepo.GetSupplierNameFromPANDO(supplierCode);
                            supplier = new Supplier(supplierCode, supplierName);
                        }

                        var ordersPerSupplier = _GeneratingOrderbooksRepo.GetOrdersPerSupplierFromPANDO(supplierCode);
                        supplier.UpdateOrderbook(ordersPerSupplier, dateTime);
                        _GeneratingOrderbooksRepo.SaveSupplier(supplier);

                        RaiseNotification(NotificationCodes.OrderbookGeneratedOK, 
                                            "Supplier (" + supplierCode.ToString() + "), ");

                        numberOfOrderbooksGenerated++;
                    }
                    catch(Exception ex)
                    {
                        overallOrderBookStatus = NotificationCodes.OrderbooksGeneratedError;
                        RaiseNotification(NotificationCodes.OrderbookGeneratedError, 
                                            "Supplier (" + supplierCode.ToString() + "), " +
                                              DomainException.FormatExceptionMessage(ex) + "  ");
                    }
                }
            }
            catch (Exception ex)
            {
                overallOrderBookStatus = NotificationCodes.OrderbooksGeneratedError;
                RaiseNotification(NotificationCodes.PandOInvalidTableAccess, 
                                    DomainException.FormatExceptionMessage(ex));
            }
            finally
            {
                try
                {
                    _GeneratingOrderbooksRepo.DeletePlannedAndOverdues();
                }
                catch(Exception ex)
                {
                    RaiseNotification(NotificationCodes.PandOTableDeletionError, 
                                        DomainException.FormatExceptionMessage(ex));
                }
                string customMessage = "";
                if (supplierCodes != null)
                {
                    customMessage = numberOfOrderbooksGenerated.ToString() + "/" +
                                        supplierCodes.Count() + " succeeded for ";
                }
                RaiseNotification(overallOrderBookStatus, customMessage);
            }
        }

        private void RaiseNotification(Guid guidStatusCode, string customMessage = "")
        {
            DomainEvents.Raise(new NotificationRaisedEvent(guidStatusCode, 
                                    DateTime.Now,
                                    customMessage));
        }

        public OrderbookPreviewsDTO GetOrderbookPreviews(string orderbookWeek = "")
        {
            OrderbookPreviewsDTO orderbookPreviewsDTO = new OrderbookPreviewsDTO();
            try
            {
                if(orderbookWeek == "")
                {
                    orderbookWeek = OrderbookWeek_VO.Create(DateTime.Now).FormatString();
                }
                var suppliers = _GeneratingOrderbooksRepo
                                        .GetSuppliersWithOrderbookForWeek(orderbookWeek);
                orderbookPreviewsDTO = DTOConversion
                                        .ConvertSuppliersToOrderbookPreviewsDTO(suppliers);
            }
            catch (Exception ex)
            {
                RaiseNotification(NotificationCodes.UnableToRetrieveOrderbookPreviews,
                                    DomainException.FormatExceptionMessage(ex));
            }
            return (orderbookPreviewsDTO);
        }

        public IReadOnlyList<OrderbookWeekDTO> GetOrderbookWeeks()
        {
            IReadOnlyList<OrderbookWeekDTO> orderbookWeekDTOs = new List<OrderbookWeekDTO>();
            try
            {
                var orderbookWeeks = _GeneratingOrderbooksRepo.GetOrderbookWeeks();
                orderbookWeekDTOs = DTOConversion.ConvertOrderbookWeeksToOrderbookWeekDTOs(orderbookWeeks);
            }
            catch(Exception ex)
            {
                RaiseNotification(NotificationCodes.UnableToRetrieveOrderbookWeeks,
                                    DomainException.FormatExceptionMessage(ex));
            }
            return (orderbookWeekDTOs);
        }

        public OrderbookDTO GetOrderbook(ulong id)
        {
            OrderbookDTO orderbookDTO = new OrderbookDTO();
            try
            {
                var orderbook = 
                    _GeneratingOrderbooksRepo
                        .GetSupplierWithOrderbookAndOrders(id);
                orderbookDTO = DTOConversion.ConvertOrderbookToOrderbookDTO(orderbook);
            }
            catch (Exception ex)
            {
                RaiseNotification(NotificationCodes.UnableToRetrieveOrderbook,
                                    DomainException.FormatExceptionMessage(ex));
            }
            return (orderbookDTO);
        }
    }
}
