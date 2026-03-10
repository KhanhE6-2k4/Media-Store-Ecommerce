namespace MediaStore.ViewModels
{
    public class OrderViewModel
    {
        public List<CartItem> cart { get; set; } = new List<CartItem>();
        public DeliveryForm deliveryInfo { get; set; } = new DeliveryForm();

        public RushOrderForm? rushOrderInfo  { get; set; }

        public int regularShippingFee { get; set; }
        public int rushShippingFee { get; set; }
    }
}