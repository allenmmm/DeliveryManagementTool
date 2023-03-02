using DMT.SharedKernel.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMT.SharedKernel
{
    public class DateTimeConfig : IDateTime
    {
        public DateTime GetTime()
        {
            return DateTime.Now;
        }
    }
}
