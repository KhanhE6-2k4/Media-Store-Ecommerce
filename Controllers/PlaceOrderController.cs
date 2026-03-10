using MediaStore.Helpers;
using MediaStore.Services.Shipping;
using MediaStore.Services.Province;
using MediaStore.Services.Session;
using MediaStore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MediaStore.Controllers
{
    public class PlaceOrderController : Controller
    {
        private readonly IShippingService _shippingService;
        private readonly IProvinceService _provinceService;
        private readonly ISessionService _sessionService;

        public PlaceOrderController(
            IShippingService shippingService,
            IProvinceService provinceService,
            ISessionService sessionService)
        {
            _shippingService = shippingService;
            _provinceService = provinceService;
            _sessionService = sessionService;
        }

        private void ClearOrderSession()
        {
            _sessionService.Remove(MySetting.DELIVERY_KEY);
            _sessionService.Remove(MySetting.RUSH_ORDER_KEY);
            _sessionService.Remove(MySetting.ORDER_KEY);
            _sessionService.Remove(MySetting.INVOICE_KEY);
        }
        public IActionResult Index()
        {
            ClearOrderSession();
            return View();
        }

        [HttpGet]
        public IActionResult DeliveryInfo()
        {
            ViewBag.ProvinceList = new SelectList(_provinceService.GetAllProvinces());
            return View();
        }

        [HttpPost]
        public IActionResult DeliveryInfo(DeliveryForm deliveryForm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ProvinceList = new SelectList(_provinceService.GetAllProvinces());
                return View(deliveryForm);
            }

            _sessionService.Set(MySetting.DELIVERY_KEY, deliveryForm);

            if (!deliveryForm.IsRushOrder)
            {
                _sessionService.Remove(MySetting.RUSH_ORDER_KEY);
            }


            return deliveryForm.IsRushOrder
                ? RedirectToAction("RushOrder", "PlaceRushOrder")
                : RedirectToAction("UpdateOrderInfo", "PlaceOrder");
        }

        public IActionResult UpdateOrderInfo()
        {
            var cart = _sessionService.Get<List<CartItem>>(MySetting.CART_KEY);
            var delivery = _sessionService.Get<DeliveryForm>(MySetting.DELIVERY_KEY);
            var rush = _sessionService.Get<RushOrderForm>(MySetting.RUSH_ORDER_KEY);
            bool hasRushOrder = rush != null;
            var order = new OrderViewModel
            {
                cart = cart,
                deliveryInfo = delivery,
                rushOrderInfo = rush,
                regularShippingFee = _shippingService.CalculateRegularFee(delivery, cart, hasRushOrder),
                rushShippingFee = _shippingService.CalculateRushFee(delivery, cart, hasRushOrder)
            };

            _sessionService.Set(MySetting.ORDER_KEY, order);
            return RedirectToAction("Invoice", "PayOrder");
        }

        public IActionResult ProcessAfterPay(string success)
        {
            return RedirectToAction("Index", "Home");
        }
    }

}