namespace MediaStore.Services.Payment
{
    public interface IPaymentFactory
    {
        string Name { get; }
        IPayment CreatePayment();
    }
}