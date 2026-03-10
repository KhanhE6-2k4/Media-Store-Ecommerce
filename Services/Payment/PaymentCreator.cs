namespace MediaStore.Services.Payment
{
    public class PaymentCreator
    {
        private readonly Dictionary<string, IPaymentFactory> _factories;

        public PaymentCreator(IEnumerable<IPaymentFactory> factories)
        {
            _factories = factories.ToDictionary(f => f.Name.ToLower());
        }

        public IPayment Get(string name)
        {
            if (_factories.TryGetValue(name.ToLower(), out var factory))
                return factory.CreatePayment();
            throw new ArgumentException("Unsupported payment method: " + name);
        }
    }
}