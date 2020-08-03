using Serverless.CommentAnalyser.Services.Abstractions;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Serverless.CommentAnalyser.Services
{
    public class TextAnalyzer : ITextAnalyzer
    {
        private HttpClient HttpClient => _httpClientFactory.CreateClient("watsonClient");

        private readonly IHttpClientFactory _httpClientFactory;
        public TextAnalyzer(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> GetCommentToneAsync(string text)
        {
            using (var response = await HttpClient.PostAsync("tone?version=2017-09-21", new StringContent(text)))
            {
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
