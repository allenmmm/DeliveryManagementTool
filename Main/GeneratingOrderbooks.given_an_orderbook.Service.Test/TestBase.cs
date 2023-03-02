using DMT.GeneratingOrderbooks.Service;
using DMT.GeneratingOrderBooks.Domain.Entities;
using DMT.GeneratingOrderBooks.Domain.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using SharedKernel.Test;
using DMT.SharedKernel.Interface;

namespace GeneratingOrderbooks.given_an_orderbook.Service.Test
{
    public abstract class TestBase
    {
        protected Mock<IDateTime> MockDateTime = new Mock<IDateTime>();       
        protected Mock<IGeneratingOrderbooksRepo> MockGeneratingOrderBooksRepo;
        protected GeneratingOrderbooksService GeneratingOrderbooksService_SUT;
        protected IReadOnlyList<PlannedAndOverdueOrder> PlannedAndOverdueOrders;
        protected PlannedAndOverdueFileParser FileParser = new PlannedAndOverdueFileParser(DateTime.Now);
        public TestBase()
        {
            FileReadUtility.FileRead(FileParser);
        }

        protected virtual void SetUp()
        {
            MockGeneratingOrderBooksRepo = new Mock<IGeneratingOrderbooksRepo>();
            GeneratingOrderbooksService_SUT = new GeneratingOrderbooksService(MockGeneratingOrderBooksRepo.Object);
            MockDateTime.Setup(fn => fn.GetTime()).Returns(DateTime.Now);
        }
    }
}
