using MediaStore.Data;
using MediaStore.ViewModels;

namespace MediaStore.Services.Order
{
    public class OrderService : IOrderService
    {
        public OrderInfo CreateOrder(OrderViewModel orderSession, InvoiceViewModel invoiceSession)
        {
            ArgumentNullException.ThrowIfNull(orderSession);
            ArgumentNullException.ThrowIfNull(invoiceSession);

            var cart = orderSession.cart;
            var deliveryInfo = orderSession.deliveryInfo;
            var rushOrderInfo = orderSession.rushOrderInfo;
            
            var delivery = new DeliveryInfo
            {
                Name = deliveryInfo.Name,
                Phone = deliveryInfo.Phone,
                Email = deliveryInfo.Email,
                Province = deliveryInfo.Province,
                Address = deliveryInfo.Address,
                Message = deliveryInfo.Message
            };

            var order = new OrderInfo
            {
                ShippingFees = orderSession.regularShippingFee + (invoiceSession.hasRushOrder ? orderSession.rushShippingFee : 0),
                Subtotal = cart.Sum(item => item.Amount),
                Status = "Pending",
                Delivery = delivery
            };

            if (rushOrderInfo != null)
            {
                order.RushOrderInfos.Add(new RushOrderInfo
                {
                    DeliveryTime = rushOrderInfo.DeliveryTime,
                    Instruction = string.IsNullOrEmpty(rushOrderInfo.Instruction) ? "No" : rushOrderInfo.Instruction,
                    Order = order
                });
            }

            if (invoiceSession.regularItem != null)
            {
                foreach (var item in invoiceSession.regularItem)
                {
                    order.OrderMedia.Add(new OrderMedia
                    {
                        MediaId = item.Id,
                        Quantity = item.Qty,
                        OrderType = 0
                    });
                }
            }

            if (invoiceSession.rushItem != null)
            {
                foreach (var item in invoiceSession.rushItem)
                {
                    order.OrderMedia.Add(new OrderMedia
                    {
                        MediaId = item.Id,
                        Quantity = item.Qty,
                        OrderType = 1
                    });
                }
            }
            
            return order;
        }
    }
}