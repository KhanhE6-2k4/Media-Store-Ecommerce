using MediaStore.Data;

namespace MediaStore.Services.Login
{
    public interface ILoginService
    {
        Task SignInAsync(User? user = null);
    }
}