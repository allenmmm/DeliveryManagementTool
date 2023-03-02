using System;
using System.Collections.Generic;
using System.Linq;

namespace DMT.DTO
{
    public class OrderbookDTO
    {
        public string SupplierName { get; private set; }
        public String OrderbookWeek { get; private set; }
        public int SupplierCode { get; private set; }
        public DateTime DateGenerated { get; private set; }
        public IReadOnlyList<OrderDTO> OrderDTOs { get; private set; }

        public OrderbookDTO()
        {
            OrderDTOs = new List<OrderDTO>();
        }

        public OrderbookDTO(string supplierName,
                            int supplierCode,
                            string orderbookWeek,
                            DateTime dateGenerated,                            
                            IEnumerable<OrderDTO> orderDTOs)
        {
            SupplierName = supplierName;
            SupplierCode = supplierCode;
            OrderbookWeek = orderbookWeek;
            DateGenerated = dateGenerated;
            OrderDTOs = orderDTOs.Select(o => new OrderDTO(o)).ToList();                                        
        }
    }
}
