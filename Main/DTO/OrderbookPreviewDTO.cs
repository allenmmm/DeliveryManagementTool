using System;

namespace DMT.DTO
{
    public class OrderbookPreviewDTO
    {
        public ulong Id { get; private set; }
        public DateTime DateGenerated { get; private set; }
        public int VendorCode { get; private set; }
        public string Description { get; private set; }

        public OrderbookPreviewDTO(){}

        public OrderbookPreviewDTO(ulong id, 
                                   DateTime dateGenerated,
                                   int vendorCode,
                                   string description)
        {
            Id = id;
            DateGenerated = dateGenerated;
            VendorCode = vendorCode;
            Description = description;
        }

        public OrderbookPreviewDTO(OrderbookPreviewDTO obp)
        {
            Id = obp.Id;
            DateGenerated = obp.DateGenerated;
            VendorCode = obp.VendorCode;
            Description = obp.Description;
        }
    }
}