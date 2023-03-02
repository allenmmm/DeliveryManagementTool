using DMT.GeneratingOrderBooks.Data;
using DMT.SharedKernel.Interface;
using Microsoft.EntityFrameworkCore;
using Moq;
using SharedKernel.Test;
using System;
using System.Linq;

namespace GeneratingOrderbooks.given_an_orderbook.Data.Test
{
    public abstract class TestBase 
    {
        protected static string conn = Guid.NewGuid().ToString();
        protected readonly PlannedAndOverdueFileParser fileParser;
        protected Mock<IDateTime> MockDateTime;

        public TestBase()
        {
            DateTime dateTime = DateTime.MinValue;
            fileParser = new PlannedAndOverdueFileParser(dateTime);
            FileReadUtility.FileRead(fileParser);
            MockDateTime = new Mock<IDateTime>();
            MockDateTime.Setup(fn => fn.GetTime())
                .Returns(fileParser.PlannedAndOverdueOrders.First().DatePulled);
        }

        protected virtual void SetUp()
        {
            GeneratingOrderbooksContext context = new GeneratingOrderbooksContext( new DbContextOptionsBuilder<DbContext>()
                    .UseInMemoryDatabase(databaseName: conn).Options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            DateTime dateTime = DateTime.MinValue;
            Seed(context, dateTime);
        }

        public void Seed(GeneratingOrderbooksContext context, DateTime dateTime)
        {
            PlannedAndOverdueFileParser fileParser = new PlannedAndOverdueFileParser(dateTime);
            FileReadUtility.FileRead(fileParser);
            using (var repo = new GeneratingOrderbooksRepository(context))
            {
                repo.UpdatePlannedAndOverdues(fileParser.PlannedAndOverdueOrders);
            }
        }
    }
}
