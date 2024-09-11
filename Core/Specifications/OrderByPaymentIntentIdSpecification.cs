using Core.Entities.OrderAggregate;
using Core.Specifications;

namespace DefaultNamespace;

public class OrderByPaymentIntentIdSpecification : BaseSpecification<Order>
{
    public OrderByPaymentIntentIdSpecification(string paymentIntentId) 
        : base(o => o.PaymentIntentId == paymentIntentId)
    {
    }
}