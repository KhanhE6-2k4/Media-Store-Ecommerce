using MediaStore.Services.Payment;
using MediaStore.Subsystem.Payment.VnPay;
namespace MediaStore.Subsystem.Payment
{
    public class VnPaySubsystem : IPayment
    {
        public void Pay(HttpContext context)
        {
            context.Response.Redirect("/VnPay/ExecutePayment");
        }
    }
}