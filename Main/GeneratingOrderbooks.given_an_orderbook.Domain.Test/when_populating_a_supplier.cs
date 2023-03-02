using DMT.GeneratingOrderBooks.Domain.ValueObjects;
using DMT.SharedKernel;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Linq;

namespace GeneratingOrderbooks.given_an_orderbook.Domain.Test
{
    [TestFixture]
    class when_populating_a_supplier : TestBase
    {
        [Test]
        public void then_create_supplier_detail_when_valid()
        {
            var supplierId_EXP = FileParser.PlannedAndOverdueOrders.First().SupplierId;
            var supplierName_EXP = FileParser.PlannedAndOverdueOrders.First().SupplierName;

            //ACT
            var supplier = Supplier_VO.Create(supplierName_EXP);
            //ASSERT
            supplier.Name.Should().Be(supplierName_EXP);
        }

        [Test]
        public void then_throw_exception_when_supplier_name_empty()
        {        
            //ACT         
            Action actionSut = () => Supplier_VO.Create("");
            //ASSERT
            actionSut.Should().Throw<DomainException>();
        }
    }
}
