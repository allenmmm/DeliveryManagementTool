using DMT.GeneratingOrderBooks.Domain.Entities;
using DMT.SharedKernel.Interface;
using System;
using System.Collections.Generic;

namespace DMT.GeneratingOrderBooks.Domain.Interfaces
{
    public interface IGeneratingOrderbooksRepo
    {
        IEnumerable<int> GetDistinctSupplierCodesFromPANDO();
        IEnumerable<String> GetOrderbookWeeks();
        IEnumerable<PlannedAndOverdueOrder> GetOrdersPerSupplierFromPANDO(int supplierCode);
        IEnumerable<Supplier> GetSuppliersWithOrderbookForWeek(String orderbookWeek);
        Supplier GetSupplierWithOrderbooks(int supplierCode);
        Supplier GetSupplierWithOrderbookAndOrders(int supplierCode, IDateTime dateTime);
        Supplier GetSupplierWithOrderbookAndOrders(ulong orderbookId);
        String GetSupplierNameFromPANDO(int supplierName);
        //Function provided only for test purposes
        void UpdatePlannedAndOverdues(IEnumerable<PlannedAndOverdueOrder> PlannedAndOverdueOrders);
        void SaveSupplier(Supplier supplier);
        void ValidatePlannedAndOverDues();
        void DeletePlannedAndOverdues();
    }
}
