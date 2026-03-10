using System.Data.Common;
using System.Threading.Tasks;
using MediaStore.Data;
using MediaStore.Helpers;
using MediaStore.Services.Session;
using MediaStore.Services.Invoices;
using MediaStore.Services.Transaction;
using MediaStore.Services.Order;
using MediaStore.Services.Email;
using MediaStore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using MediaStore.Services.Payment;

namespace MediaStore.Controllers
{
    public class PayOrderController : Controller
    {
        private readonly ISessionService _sessionService;
        private readonly IInvoiceService _invoiceService;
        private readonly ITransactionService _transactionService;
        private readonly IOrderService _orderService;
        private readonly IEmailService _emailService;
        private readonly PaymentCreator _paymentCreator;

        public PayOrderController(
            ISessionService sessionService,
            IInvoiceService invoiceBuilder,
            ITransactionService transactionService,
            IOrderService orderService,
            IEmailService emailService,
            PaymentCreator paymentCreator)
        {
            _sessionService = sessionService;
            _invoiceService = invoiceBuilder;
            _transactionService = transactionService;
            _orderService = orderService;
            _emailService = emailService;
            _paymentCreator = paymentCreator;
        }

        public IActionResult Invoice()
        {
            var order = _sessionService.Get<OrderViewModel>(MySetting.ORDER_KEY);
            var invoice = _invoiceService.CreateInvoice(order);

            _sessionService.Set(MySetting.INVOICE_KEY, invoice);

            if (invoice.hasRushOrder)
                return RedirectToAction("RushInvoice");

            return View(invoice);
        }

        public IActionResult RushInvoice()
        {
            var invoice = _sessionService.Get<InvoiceViewModel>(MySetting.INVOICE_KEY);
            return View(invoice);
        }

        public IActionResult PaymentSuccess() => View();

        public IActionResult PaymentFail() => View();

        public async Task<IActionResult> PaymentResult()
        {
            var msg = TempData.Peek("Message")?.ToString();
            var success = TempData.Peek("Success")?.ToString();

            TempData.Keep("Message");
            TempData.Keep("Success");

            if (success == "no")
                return RedirectToAction("PaymentFail");

            TempData["Message"] = msg + " Please check your email for order's information";

            var transaction = _sessionService.Get<PaymentTransaction>(MySetting.TRANSACTION_KEY);
            var orderSession = _sessionService.Get<OrderViewModel>(MySetting.ORDER_KEY);
            var invoiceSession = _sessionService.Get<InvoiceViewModel>(MySetting.INVOICE_KEY);

            if (transaction != null)
            {
                await _transactionService.SaveTransactionAsync(transaction);
            }

            if (orderSession != null && invoiceSession != null)
            {
                var order = _orderService.CreateOrder(orderSession, invoiceSession);
                var invoice = new Invoice
                {
                    TotalAmount = invoiceSession.TotalPrice,
                    Transaction = transaction,
                    Order = order
                };
                await _invoiceService.SaveInvoiceAsync(invoice);

                // Fire-and-forget: do not await email to avoid blocking the response
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _emailService.SendOrderConfirmationEmail(
                            invoiceSession.email,
                            order.OrderId.ToString(),
                            invoiceSession.name);
                    }
                    catch { /* email failure should not crash the flow */ }
                });
            }

            _sessionService.Clear();

            return RedirectToAction("PaymentSuccess");
        }
        public IActionResult RedirectToPayment(string paymentMethod)
        {
            IPayment payment = _paymentCreator.Get(paymentMethod); // Lấy phương thức thanh toán tương ứng
            payment.Pay(HttpContext);
            return new EmptyResult();       
        }
    }
}