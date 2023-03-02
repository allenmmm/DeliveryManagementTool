using DMT.GeneratingOrderBooks.Domain.Interfaces;
using DMT.SharedKernel;

namespace DMT.GeneratingOrderBooks.Domain.ValueObjects
{
    public class Part_VO : ValueObject<Part_VO>
    {
        public string Number { get; private set; }
        public string Description { get; private set; }

        private Part_VO(IPart part)
        {
            Number = part.PartNumber;
            Description = part.PartDescription;
        }
        private Part_VO(){}

        public static Part_VO Create(IPart part)
        {
            Gaurd.AgainstNull(part,"part value object is null");
            Gaurd.AgainstNull(part.PartNumber, "Part number null");
            Gaurd.AgainstEmpty(part.PartNumber, "Part number must be provided");
            Gaurd.AgainstNull(part.PartDescription, "Part description null");
            Gaurd.AgainstEmpty(part.PartDescription, "Part description must be provided");
            return new Part_VO(part);
        }
    }
}
