using MediaStore.Data;
namespace MediaStore.Services.Transaction
{
    public class TransactionService : ITransactionService
    {
        private readonly AimsContext _db;

        public TransactionService(AimsContext db)
        {
            _db = db;
        }

        public async Task SaveTransactionAsync(PaymentTransaction transaction)
        {
            _db.PaymentTransactions.Add(transaction);
            await _db.SaveChangesAsync();
        }
    }
}