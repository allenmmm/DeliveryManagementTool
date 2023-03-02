using DMT.SharedKernel;

namespace DMT.GeneratingOrderBooks.Domain.ValueObjects
{
    public class Supplier_VO : ValueObject<Supplier_VO>
    {
        public string Name { get; private set; } 

        private Supplier_VO(string name)
        {
            Name = name;
        }

        public static Supplier_VO Create(string name)
        {

            Gaurd.AgainstNull(name,"Supplier name null");
            Gaurd.AgainstEmpty(name, "Supplier name must be provided");
            return new Supplier_VO(name);
        }
    }
}
