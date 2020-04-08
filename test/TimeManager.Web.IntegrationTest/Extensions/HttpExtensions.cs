using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TimeManager.Web.Models.Account;
using Xunit;

namespace TimeManager.Web.IntegrationTest.Extensions
{
    public static class HttpExtensions
    {
        public static Task<HttpResponseMessage> PostAsync<T>(this HttpClient httpClient, string url, T content)
        {
            return httpClient.PostAsync(url, new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json"));
        }

        public static async Task<T> GetAsync<T>(this HttpClient httpClient, string url)
        {
            var responseMessage = await httpClient.GetAsync(url);
            return await responseMessage.ReadContentAsync<T>();
        }

        public static Task<HttpResponseMessage> PutAsync<T>(this HttpClient httpClient, string url, T content)
        {
            return httpClient.PutAsync(url, new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json"));

        }

        public static async Task<T> ReadContentAsync<T>(this HttpResponseMessage responseMessage)
        {
            string contentText = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<T>(contentText);
        }

        public static async Task AuthAs(this HttpClient httpClient, string email, string password)
        {
            var request = new SignInRequest
            {
                UserName = email,
                Password = password,
                RememberMe = true
            };

            var responseMessage = await httpClient.PostAsync("/api/Account/SignIn", request);
            Assert.True(responseMessage.IsSuccessStatusCode);
        }
    }
}
