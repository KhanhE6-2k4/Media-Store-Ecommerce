using MediaStore.Exceptions;
using MediaStore.Helpers;
using MediaStore.Services.Session;
using MediaStore.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MediaStore.Controllers
{
    public class PlaceRushOrderController : Controller
    {
        private readonly ISessionService _sessionService;

        public PlaceRushOrderController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }
        [HttpGet]
        public IActionResult RushOrder()
        {
            try
            {
                var deliveryInfo = _sessionService.Get<DeliveryForm>(MySetting.DELIVERY_KEY);
                var cart = _sessionService.Get<List<CartItem>>(MySetting.CART_KEY);
                if (deliveryInfo.Province != "Hà Nội")
                {
                    throw new DeliveryAddressNoSupportException("The current delivery address doesn't support this service. Please update the delivery information!");
                }
                var rushableItem = cart.Any(item => item.IsRushOrderSupported == true);
                if (!rushableItem)
                {
                    throw new NoRushOrderItemExceptionException("The current cart doesn't have any product that supported rush order. Please update the cart or delivery method!"); ;
                }
                // Mac dinh goi y giao sau 2h
                var rushForm = new RushOrderForm
                {
                    DeliveryTime = DateTime.Now.AddHours(2)
                };
                return View(rushForm);

            }
            catch (OrderException ex)
            {
                TempData["OrderError"] = ex.Message;
                return RedirectToAction("DeliveryInfo", "PlaceOrder");
            }
        }

        [HttpPost]
        public IActionResult RushOrder(RushOrderForm rushForm)
        {
            var now = DateTime.Now;
            if (!rushForm.DeliveryTime.HasValue)
            {
                ModelState.AddModelError(nameof(rushForm.DeliveryTime), "Please select a delivery time.");
            }
            else if (rushForm.DeliveryTime < now.AddHours(2))
            {
                ModelState.AddModelError(nameof(rushForm.DeliveryTime), "Delivery time must be at least 2 hours from now.");
            }
            else if (rushForm.DeliveryTime > now.AddDays(3))
            {
                ModelState.AddModelError(nameof(rushForm.DeliveryTime), "Delivery time must be within the next 3 days.");
            }
            if (!ModelState.IsValid)
            {
                SetDeliveryTimeLimit();
                return View(rushForm);
            }
            HttpContext.Session.Set<RushOrderForm>(MySetting.RUSH_ORDER_KEY, rushForm);
            return RedirectToAction("UpdateOrderInfo", "PlaceOrder");
        }

        private void SetDeliveryTimeLimit()
        {
            ViewBag.MinDeliveryTime = DateTime.Now.AddHours(2).ToString("yyyy-MM-ddTHH:mm");
            ViewBag.MaxDeliveryTime = DateTime.Now.AddDays(3).ToString("yyyy-MM-ddTHH:mm");
        }



    }
}