using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SkyPathWSClient
{
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
        public void SetQueryParameter(string key, string value)
        {
            if (this.uriBuilder.Query == string.Empty)
            {
                this.uriBuilder.Query += "?";
            }
            else this.uriBuilder.Query += "&";
            this.uriBuilder.Query += $"{key}={value}";
        }
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
                        T model = JsonSerializer.Deserialize<T>(result, jsonSerializerOptions);
                        return model;
                    }
                    return default(T);
                }
            }
        }

        public async Task<bool> PostAsync(T model, List<FileStream> files)
        {
            using (HttpRequestMessage httpRequest = new HttpRequestMessage())
            {
                httpRequest.Method = HttpMethod.Post;
                httpRequest.RequestUri = this.uriBuilder.Uri;
                MultipartFormDataContent multipartFormData = new MultipartFormDataContent();
                string jsonModel = JsonSerializer.Serialize<T>(model);
                StringContent ModelContent = new StringContent(jsonModel);
                multipartFormData.Add(ModelContent, "model");
                foreach(FileStream fileStream in files)
                {
                    StreamContent streamContent = new StreamContent(fileStream);
                    multipartFormData.Add(streamContent, "file", fileStream.Name);
                }
                httpRequest.Content = multipartFormData;
                using (HttpResponseMessage responseMessage = await this.httpClient.SendAsync(httpRequest))
                {
                    return responseMessage.IsSuccessStatusCode == true;
                }
            }
        }
    }
}
