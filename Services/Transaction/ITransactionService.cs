using MediaStore.Data;
namespace MediaStore.Services.Transaction
{
    public interface ITransactionService
    {
        Task SaveTransactionAsync(PaymentTransaction transaction);
    }

}