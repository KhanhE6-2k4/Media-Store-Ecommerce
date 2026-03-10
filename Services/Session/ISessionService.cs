using Microsoft.AspNetCore.Http;
using System.Text.Json; // JsonSerializer trong session extension

namespace MediaStore.Services.Session
{
    public interface ISessionService
    {
        T Get<T>(string key);
        void Set<T>(string key, T value);

        void Remove(string key);
        void Clear();
    }
}