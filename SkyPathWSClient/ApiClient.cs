using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SkyPathWSClient
{
    // Small helper used by the website and the WPF admin app to call the Web API.
    // You set Scheme/Host/Port/Path, then call GetAsync or PostAsync.
    // <T> is the type we expect back (for example List<Flight> or User).
    public class ApiClient<T>
    {
        HttpClient httpClient = SkyPathHttpClient.Instance;
        UriBuilder uriBuilder = new UriBuilder();

        public string Scheme
        {
            set
            {
                this.uriBuilder.Scheme = value;
            }
        }
        public string Host
        {
            set
            {
                this.uriBuilder.Host = value;
            }
        }
        public int Port
        {
            set
            {
                this.uriBuilder.Port = value;
            }
        }
        public string Path
        {
            set
            {
                this.uriBuilder.Path = value;
            }
        }


        // Adds "?key=value" (or "&key=value") to the URL, e.g. ?user_id=5
        public void SetQueryParameter(string key, string value)
        {
            if (this.uriBuilder.Query == string.Empty)
            {
                this.uriBuilder.Query += "?";
            }
            else this.uriBuilder.Query += "&";
            this.uriBuilder.Query += $"{key}={value}";
        }
        // Sends a GET request and turns the JSON response into an object of type T.
        public async Task<T> GetAsync()
        {
            using (HttpRequestMessage httpRequest = new HttpRequestMessage())
            {
                httpRequest.Method = HttpMethod.Get;
                httpRequest.RequestUri = this.uriBuilder.Uri;
                using (HttpResponseMessage httpResponse = await this.httpClient.SendAsync(httpRequest))
                {
                    if (httpResponse.IsSuccessStatusCode == true)
                    {
                        string result = await httpResponse.Content.ReadAsStringAsync();
                        JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions();
                        jsonSerializerOptions.PropertyNameCaseInsensitive = true;
                        // Convert the JSON text from the API into a C# object of type T.
                        T model = JsonSerializer.Deserialize<T>(result, jsonSerializerOptions);
                        return model;
                    }
                    return default(T);
                }
            }
        }

        // Holds the API's error text when the last PostAsync failed (used to show the reason to the user).
        public string LastError { get; private set; }
        // Sends an object as JSON with POST. Returns true if the API accepted it.
        // If it failed, the reason is stored in LastError.
        public async Task<bool> PostAsync(T model)
        {
            using (HttpRequestMessage httpRequest = new HttpRequestMessage())
            {
                httpRequest.Method = HttpMethod.Post;
                httpRequest.RequestUri = this.uriBuilder.Uri;

                // Turn the C# object into JSON text to send in the request body.
                string json = JsonSerializer.Serialize<T>(model);
                httpRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");

                using (HttpResponseMessage httpResponse = await this.httpClient.SendAsync(httpRequest))
                {
                    string responseBody = await httpResponse.Content.ReadAsStringAsync();

                    if (!httpResponse.IsSuccessStatusCode)
                    {
                        LastError = responseBody?.Trim('"');
                        Console.WriteLine($"POST failed: {(int)httpResponse.StatusCode} {httpResponse.ReasonPhrase}");
                        Console.WriteLine(responseBody);
                        return false;
                    }

                    if (bool.TryParse(responseBody, out bool apiResult))
                        return apiResult;

                    // success status with empty/non-bool body still counts as success
                    return true;
                }
            }
        }


        public async Task<bool> PostAsync(T model, Stream file)
        {
            using (HttpRequestMessage httpRequest = new HttpRequestMessage())
            {
                httpRequest.Method = HttpMethod.Post;
                httpRequest.RequestUri = this.uriBuilder.Uri;
                MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
                string json = JsonSerializer.Serialize<T>(model);

                StringContent modelContent = new StringContent(json);
                multipartFormDataContent.Add(modelContent, "model");

                StreamContent streamContent = new StreamContent(file); // Streamcontent because of the file stream
                multipartFormDataContent.Add(streamContent, "file", "file");

                httpRequest.Content = multipartFormDataContent;
                using (HttpResponseMessage httpResponse = await this.httpClient.SendAsync(httpRequest))
                {
                    return httpResponse.IsSuccessStatusCode;
                }

            }

        }


        public async Task<bool> PostAsync(T model, List<Stream> files)
        {
            using (HttpRequestMessage httpRequest = new HttpRequestMessage())
            {
                httpRequest.Method = HttpMethod.Post;
                httpRequest.RequestUri = this.uriBuilder.Uri;
                MultipartFormDataContent multipartFormData = new MultipartFormDataContent();
                string jsonModel = JsonSerializer.Serialize<T>(model);
                StringContent ModelContent = new StringContent(jsonModel);
                multipartFormData.Add(ModelContent, "model");
                foreach (Stream Stream in files)
                {
                    StreamContent streamContent = new StreamContent(Stream);
                    multipartFormData.Add(streamContent, "file", "file");
                }
                httpRequest.Content = multipartFormData;
                using (HttpResponseMessage responseMessage = await this.httpClient.SendAsync(httpRequest))
                {
                    return responseMessage.IsSuccessStatusCode == true;
                }
            }
        }

        // Like PostAsync, but also reads a JSON object back from the API.
        // Example: send a LoginViewModel, get a User in return.
        public async Task<TResponse> PostAsyncReturn<TRequest, TResponse>(TRequest model)
        {

            using (HttpRequestMessage httpRequest = new HttpRequestMessage())
            {
                httpRequest.Method = HttpMethod.Post;
                httpRequest.RequestUri = this.uriBuilder.Uri;
                string json = JsonSerializer.Serialize<TRequest>(model);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                httpRequest.Content = content;
                using (HttpResponseMessage httpResponse = await this.httpClient.SendAsync(httpRequest))
                {
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        string result = await httpResponse.Content.ReadAsStringAsync();

                        if (string.IsNullOrWhiteSpace(result))
                            return default(TResponse);
                        JsonSerializerOptions options = new JsonSerializerOptions();
                        options.PropertyNameCaseInsensitive = true;
                        TResponse value = JsonSerializer.Deserialize<TResponse>(result, options);
                        return value;
                    }
                }
                return default(TResponse);

            }
        }
    }
}
