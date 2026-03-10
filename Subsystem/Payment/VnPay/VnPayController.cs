using MediaStore.Data;
using MediaStore.Helpers;
using MediaStore.Services;
using MediaStore.Services.Payment.Vnpay;
using MediaStore.Services.Session;
using MediaStore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace MediaStore.Subsystem.Payment.VnPay
{
    public class VnPayController : Controller
    {
        private readonly IVnPayService _vnPayService;

        private readonly ISessionService _sessionService;

        public VnPayController(IVnPayService vnPayService, ISessionService sessionService)
        {
            _vnPayService = vnPayService;
            _sessionService = sessionService;
        }
        public IActionResult ExecutePayment()
        {
            var invoice = _sessionService.Get<InvoiceViewModel>(MySetting.INVOICE_KEY);
            var vnPaymentRequest = _vnPayService.CreateRequest(invoice);
            string paymentURl = _vnPayService.CreatePaymentUrl(HttpContext, vnPaymentRequest);
            return Redirect(paymentURl);
        }
        public IActionResult PaymentCallBack()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            if (response == null)
            {
                TempData["Message"] = $"Unknown error.";
                return RedirectToAction("PaymentResult", "PayOrder");
            }
            var paymentTransaction = new PaymentTransaction
            {
                PaymentTime = ParseVnpDate(response.PayDate),
                PaymentAmount = response.Amount,
                Content = response.OrderDescription,
                BankTransactionId = response.BankCode,
                CardType = response.CardType
            };
            _sessionService.Set<PaymentTransaction>(MySetting.TRANSACTION_KEY, paymentTransaction);
            var responseCode = response.VnPayResponseCode;
            TempData["Message"] = Message.GetMessage(responseCode);
            TempData["Success"] = responseCode == "00" ? "yes" : "no"; 
            return RedirectToAction("PaymentResult", "PayOrder");
        }
        private DateTime ParseVnpDate(string vnpDateString)
        {
            return DateTime.ParseExact(vnpDateString, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        }

    }
}