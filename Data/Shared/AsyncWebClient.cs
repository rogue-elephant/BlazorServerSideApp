using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class AsyncWebClient : IDisposable
{
    HttpClient HttpClient { get; set; }

    public AsyncWebClient()
    {
        this.HttpClient = new HttpClient();
        this.HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("C# app");
    }
    public void Dispose() => this.HttpClient?.Dispose();

    public async Task<T> Get<T>(string url, CancellationToken cancellationToken)
    {
        using (this.HttpClient)
        using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, url))
        using (var httpResponse = await this.HttpClient
            .SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
        {
            var stream = await httpResponse.Content.ReadAsStreamAsync();

            if (httpResponse.IsSuccessStatusCode)
                return DeserializeJsonFromStream<T>(stream);

            var responseContent = await StreamToStringAsync(stream);
            throw new AsyncWebClientException
            {
                StatusCode = (int)httpResponse.StatusCode,
                Content = responseContent
            };
        }
    }

    private T DeserializeJsonFromStream<T>(Stream stream)
    {
        if (stream == null || stream.CanRead == false)
            return default(T);

        using (var streamReader = new StreamReader(stream))
        using (var jsonTextReader = new JsonTextReader(streamReader))
            try
            {
                return new JsonSerializer().Deserialize<T>(jsonTextReader);
            }
            catch (System.Exception)
            {
                return default(T);
            }
    }

    public async Task<string> StreamToStringAsync(Stream stream)
    {
        if (stream == null || stream.CanRead == false)
            return null;

        using (var streamReader = new StreamReader(stream))
            return await streamReader.ReadToEndAsync();
    }
}

public class AsyncWebClientException : Exception
{
    public int StatusCode { get; set; }

    public string Content { get; set; }
}