using DMT.GeneratingOrderBooks.Domain.ValueObjects;
using System;

namespace DMT.GeneratingOrderBooks.Domain.Entities
{
    public class Order
    {
        public String PandOId { get; private set; }
        public ulong OrderbookId { get;  private set; }
        public Order_VO Details { get; private set; }
        public Part_VO Part { get; private set;  }

        public Order(PlannedAndOverdueOrder plannedAndOverdueOrder) 
            : base()
        {
            PandOId = plannedAndOverdueOrder.Id;
            Details = Order_VO.Create(plannedAndOverdueOrder);
            Part = Part_VO.Create(plannedAndOverdueOrder);
        }

        private Order() { }

        // For simple entities, this may suffice
        // As Evans notes earlier in the course, equality of Entities is frequently not a simple operation
        public override bool Equals(object otherObject)
        {
            var entity = otherObject as Order;
            if (entity != null)
            {
                return this.Equals(entity);
            }
            return base.Equals(otherObject);
        }

        public override int GetHashCode()
        {
            return this.PandOId.GetHashCode();
        }

        public bool Equals(Order other)
        {
            if (other == null)
            {
                return false;
            }
            return (this.PandOId.Equals(other.PandOId) &&
                        this.OrderbookId.Equals(other.OrderbookId));
        }
    }
}
