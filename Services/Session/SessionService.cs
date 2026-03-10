using Microsoft.AspNetCore.Http;
using System.Text.Json; // JsonSerializer trong session extension
using MediaStore.Helpers;
namespace MediaStore.Services.Session
{
    public class SessionService : ISessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public T Get<T>(string key)
        {
            return _httpContextAccessor.HttpContext.Session.Get<T>(key);
        }

        public void Set<T>(string key, T value)
        {
            _httpContextAccessor.HttpContext.Session.Set(key, value);
        }

        public void Remove(string key)
        {
            _httpContextAccessor.HttpContext.Session.Remove(key);
        }

        public void Clear()
        {
            _httpContextAccessor.HttpContext.Session.Clear();
        }

       
    }
}