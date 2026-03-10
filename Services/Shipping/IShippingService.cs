using MediaStore.ViewModels;

namespace MediaStore.Services.Shipping
{
    public interface IShippingService
    {
        List<CartItem> GetRegularItem(List<CartItem> cart, bool hasRushOrder);

        List<CartItem> GetRushItem(List<CartItem> cart, bool hasRushOrder);
        int CalculateRegularFee(DeliveryForm form, List<CartItem> cart, bool hasRushOrder);
        int CalculateRushFee(DeliveryForm form, List<CartItem> cart, bool hasRushOrder);
    }
}