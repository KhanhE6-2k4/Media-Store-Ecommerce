using MediaStore.Models;
using MediaStore.ViewModels;
using NuGet.Protocol.Plugins;

namespace MediaStore.Services.Shipping
{
    public class ShippingService : IShippingService
    {
        public List<CartItem> GetRegularItem(List<CartItem> cart, bool hasRushOrder)
        {
            List<CartItem> regularItem = new List<CartItem>();
            if (hasRushOrder)
            {
                foreach (var item in cart)
                {
                    regularItem.Add(item);
                }
                return regularItem;
            }
            return cart;
        }

        public List<CartItem> GetRushItem(List<CartItem> cart, bool hasRushOrder)
        {
            List<CartItem> rushItem = new List<CartItem>();

            if (hasRushOrder)
            {
                foreach (var item in cart)
                {
                    if (item.IsRushOrderSupported)
                    {
                        rushItem.Add(item);
                    }
                }
                return rushItem;
            }
            return null;
        }
        public int CalculateRegularFee(DeliveryForm form, List<CartItem> cart, bool hasRushOrder)
        {
            var regularItem = GetRegularItem(cart, hasRushOrder);
            if (regularItem == null)
                return 0;
            double heaviestItem = regularItem.Max(item => item.Weight);
            int SubTotal = regularItem.Sum(item => item.Amount);
            int regularFee = 0;
            string province = form.Province;
            if (province == "Hà Nội" || province == "Hồ Chí Minh")
            {
                regularFee = 22000;
                if (heaviestItem > 3.0)
                    regularFee += (int)(Math.Ceiling((heaviestItem - 3.0) / 0.5) * 2500);
            }
            else
            {
                regularFee = 30000;
                if (heaviestItem > 0.5)
                    regularFee += (int)(Math.Ceiling((heaviestItem - 0.5) / 0.5) * 2500);
            }

            if (SubTotal > 100000)
                regularFee -= Math.Min(25000, regularFee);

            regularFee = Math.Max(regularFee, 0);
            return regularFee;
        }
        public int CalculateRushFee(DeliveryForm form, List<CartItem> cart, bool hasRushOrder)
        {
            var rushItem = GetRushItem(cart, hasRushOrder);
            if (rushItem == null)
                return 0;
            var rushFee = rushItem.Count * 10000;
            return rushFee;
        }
    }

}