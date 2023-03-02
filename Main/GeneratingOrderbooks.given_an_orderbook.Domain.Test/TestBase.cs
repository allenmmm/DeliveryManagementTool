using DMT.SharedKernel.Interface;
using Moq;
using SharedKernel.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratingOrderbooks.given_an_orderbook.Domain.Test
{
    public abstract class TestBase
    {
        protected readonly PlannedAndOverdueFileParser FileParser;
        protected readonly Mock<IDateTime> MockDateTime;
        public TestBase()
        {
            DateTime dateTime = DateTime.MinValue;
            FileParser = new PlannedAndOverdueFileParser(dateTime);
            FileReadUtility.FileRead( FileParser);
            MockDateTime = new Mock<IDateTime>();
            MockDateTime.Setup(fn => fn.GetTime())
                .Returns(FileParser.PlannedAndOverdueOrders.First().DatePulled);
        }
    }
}
