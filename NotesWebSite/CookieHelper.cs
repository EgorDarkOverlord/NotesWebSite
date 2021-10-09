using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace NotesWebSite
{
    public static class CookieHelper
    {
        public static void SerializeObjectAsJson<T>(this IResponseCookies cookies, string key, T value)
        {
            cookies.Append(key, JsonSerializer.Serialize(value));
        }

        public static void RewriteObjectAsJson<T>(this IResponseCookies cookies, string key, T value)
        {
            cookies.Delete(key);
            cookies.Append(key, JsonSerializer.Serialize(value));
        }

        public static T DeserializeObjectFromJson<T>(this IRequestCookieCollection cookies, string key)
        {
            var value = cookies[key];
            return value == null ? default(T) : JsonSerializer.Deserialize<T>(value);
        }

    }
}
