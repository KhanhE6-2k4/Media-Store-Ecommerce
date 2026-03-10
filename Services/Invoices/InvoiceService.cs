using MediaStore.Data;
using MediaStore.ViewModels;
namespace MediaStore.Services.Invoices
{
    public class InvoiceService : IInvoiceService
    {
        private readonly AimsContext _db;

        public InvoiceService(AimsContext db)
        {
            _db = db;
        }

        public async Task SaveInvoiceAsync(Invoice invoice)
        {
            _db.Invoices.Add(invoice);
            await _db.SaveChangesAsync();
        }

        public InvoiceViewModel CreateInvoice(OrderViewModel order)
        {
            var cart = order.cart;
            var deliveryInfo = order.deliveryInfo;

            if (deliveryInfo.IsRushOrder)
            {
                var regularItem = new List<CartItem>();
                var rushItem = new List<CartItem>();
                int regularSubTotal = 0, rushSubTotal = 0;

                foreach (var item in cart)
                {
                    if (item.IsRushOrderSupported)
                    {
                        rushItem.Add(item);
                        rushSubTotal += item.Amount;
                    }
                    else
                    {
                        regularItem.Add(item);
                        regularSubTotal += item.Amount;
                    }
                }

                return new InvoiceViewModel
                {
                    name = deliveryInfo.Name,
                    phone = deliveryInfo.Phone,
                    address = deliveryInfo.Address + " - " + deliveryInfo.Province,
                    email = deliveryInfo.Email,
                    hasRushOrder = true,
                    regularItem = regularItem,
                    regularSubTotal = regularSubTotal,
                    regularShippingFee = order.regularShippingFee,
                    rushItem = rushItem,
                    rushSubTotal = rushSubTotal,
                    rushShippingFee = order.rushShippingFee
                };
            }

            return new InvoiceViewModel
            {
                name = deliveryInfo.Name,
                phone = deliveryInfo.Phone,
                address = deliveryInfo.Address + " - " + deliveryInfo.Province,
                email = deliveryInfo.Email,
                hasRushOrder = false,
                regularItem = cart,
                regularSubTotal = cart.Sum(item => item.Amount),
                regularShippingFee = order.regularShippingFee
            };

        }

    }
}