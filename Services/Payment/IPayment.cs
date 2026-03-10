namespace MediaStore.Services.Payment
{
    public interface IPayment
    {
        void Pay(HttpContext context);
    }
}