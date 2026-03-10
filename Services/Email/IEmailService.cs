namespace MediaStore.Services.Email
{
    public interface IEmailService
    {
        Task SendOrderConfirmationEmail(string toEmail, string orderCode, string customerName);
    }
}