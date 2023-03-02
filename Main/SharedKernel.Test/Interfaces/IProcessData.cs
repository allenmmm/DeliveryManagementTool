using DMT.SharedKernel.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Test.Interfaces
{
    public interface IProcessData
    {
        void ProcessLine(List<String> rawData);
    }
}
