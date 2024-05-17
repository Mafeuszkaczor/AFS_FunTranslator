using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AFS.Services
{
    public class FunTranslationsService : IFunTranslationsService
    {
        private readonly HttpClient _httpClient;

        public FunTranslationsService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("FunTranslationsApi");
        }

        public async Task<string> TranslateAsync(string text, string translationType)
        {
            var response = await _httpClient.GetAsync($"translate/{translationType}.json?text={Uri.EscapeDataString(text)}");
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            return jsonResponse;
        }
    }
}
