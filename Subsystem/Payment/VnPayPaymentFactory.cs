using MediaStore.Services.Payment;
using MediaStore.Subsystem.Payment.VnPay;


namespace MediaStore.Subsystem.Payment
{
    public class VnPayPaymentFactory : IPaymentFactory
    {
        public string Name => "vnpay";
        public IPayment CreatePayment()
        {
            return new VnPaySubsystem();
        }
    }
}