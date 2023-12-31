using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace BizsolTech.Chatbot.Helpers
{
    public class CBApiRequestHandler
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _baseUrl;

        public CBApiRequestHandler(IHttpClientFactory httpClientFactory, string baseUrl)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
        }

        public async Task<(string ResponseContent, HttpStatusCode StatusCode)> GetAsync(string apiUrl, Dictionary<string, string>? parameters = null)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var fullUrl = BuildUrlWithParameters(_baseUrl + apiUrl, parameters);

                var response = await client.GetAsync(fullUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    return (responseData, response.StatusCode);
                }
                else
                {
                    return (string.Empty, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error executing GET request", ex);
            }
        }

        public async Task<(string ResponseContent, HttpStatusCode StatusCode)> PostAsync(string apiUrl, object requestBody, Dictionary<string, string>? parameters = null)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var fullUrl = BuildUrlWithParameters(_baseUrl + apiUrl, parameters);

                var jsonRequest = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(fullUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    return (responseData, response.StatusCode);
                }
                else
                {
                    return (string.Empty, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error executing POST request", ex);
            }
        }

        public async Task<(string ResponseContent, HttpStatusCode StatusCode)> PutAsync(string apiUrl, object requestBody, Dictionary<string, string>? parameters = null)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var fullUrl = BuildUrlWithParameters(_baseUrl + apiUrl, parameters);

                var jsonRequest = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                var response = await client.PutAsync(fullUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    return (responseData, response.StatusCode);
                }
                else
                {
                    return (string.Empty, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error executing PUT request", ex);
            }
        }

        private string BuildUrlWithParameters(string url, Dictionary<string, string>? parameters)
        {
            if (parameters == null || parameters.Count == 0)
            {
                return url;
            }

            var queryString = new StringBuilder("?");
            foreach (var kvp in parameters)
            {
                queryString.Append($"{kvp.Key}={kvp.Value}&");
            }
            queryString.Length--; // Remove the last '&'

            return url + queryString.ToString();
        }
    }
}
