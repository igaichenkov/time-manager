using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TimeManager.IntegrationTest.Extensions
{
    public static class HttpExtensions
    {
        public static Task<HttpResponseMessage> PostAsync<T>(this HttpClient httpClient, string url, T content)
        {
            return httpClient.PostAsync(url, new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json"));
        }

        public static async Task<T> ReadContentAsync<T>(this HttpResponseMessage responseMessage)
        {
            string contentText = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<T>(contentText);
        }
    }
}
