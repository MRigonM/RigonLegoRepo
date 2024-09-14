using System.Runtime.Serialization;

namespace Core.Entities.OrderAggregate
{
    public enum OrderStatus
    {
        [EnumMember(Value = "Pending")]
        Pending,
        [EnumMember(Value = "Payment Recived")]
        PaymentRecived,
        [EnumMember(Value = "Payment Failed")]
        PaymentFailed
    }
}