using MediaStore.Data;
using MediaStore.ViewModels;
namespace MediaStore.Services.Invoices
{
    public interface IInvoiceService
    {
        Task SaveInvoiceAsync(Invoice invoice);

        InvoiceViewModel CreateInvoice(OrderViewModel orderSession);

    }
}