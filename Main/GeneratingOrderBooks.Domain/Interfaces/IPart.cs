using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.GeneratingOrderBooks.Domain.Interfaces
{
    public interface IPart
    {
        string PartNumber { get; }
        string PartDescription { get; }
    }
}
