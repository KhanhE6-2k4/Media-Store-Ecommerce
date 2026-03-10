using MediaStore.Subsystem.Payment.VnPay;
using MediaStore.ViewModels;

namespace MediaStore.Services.Payment.Vnpay
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collections);

        VnPaymentRequestModel CreateRequest(InvoiceViewModel invoice);
    }
}