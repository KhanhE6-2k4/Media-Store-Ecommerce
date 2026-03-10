using MediaStore.Data;
using MediaStore.ViewModels;

namespace MediaStore.Services.Order
{
    public interface IOrderService
    {
        OrderInfo CreateOrder(OrderViewModel orderSession, InvoiceViewModel invoiceSession);
    }
}