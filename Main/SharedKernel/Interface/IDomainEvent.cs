using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.SharedKernel.Interface
{
    public interface IDomainEvent
    {
        DateTime DateTimeEventFired { get; }
    }
}
