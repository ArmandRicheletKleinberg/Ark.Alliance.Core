using Ark.Net.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using JsonException = System.Text.Json.JsonException;
using JsonSerializer = System.Text.Json.JsonSerializer;

// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable UnusedMember.Global

namespace Ark.Net.Http
{
    /// <summary>
    /// This is the base class to all HTTP REST API service client repository.
    /// </summary>
    public abstract class HttpRepositoryBase
    {
        #region Properties (Properties)

        /// <summary>
        /// The default request time out is 15 seconds but can be changed in the inheriting classes.
        /// </summary>
        protected virtual TimeSpan Timeout
            => TimeSpan.FromSeconds(15);

        #endregion Properties (Properties)

        #region Methods (Virtual)

        /// <summary>
        /// Requests to GET some data on an HTTP REST Service given a relative path and some optional HTTP query strings, headers.
        /// It also deserialize the HTTP response and returns a Result depending on the HTTP code returned.
        /// This request is to be made on our custom HTTP REST server as it only react to some HTTP code.
        /// </summary>
        /// <typeparam name="TResponseData">The expected type of HTTP response data received.</typeparam>
        /// <param name="urlRelativePath">The relative path of the URL. It will be concatenated with the RootUrl and also the query strings if any.</param>
        /// <param name="queryParameters">The query string parameters if any. They will be appended to the absolute URL.</param>
        /// <param name="headers">The HTTP headers to set if any.</param>
        /// <returns>
        /// Success : The request has succeeded and the response was successfully deserialized into the result response type.
        /// BadPrerequisites : The RootUrl has not been set.
        /// NoConnection : Unable to connect the HTTP REST server.
        /// Unauthorized : The service requested is unauthorized or forbidden.
        /// Failure : The response deserialization has failed.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        protected virtual Task<Result<TResponseData>> Get<TResponseData>(string urlRelativePath, Dictionary<string, object> queryParameters = null, Dictionary<string, string> headers = null)
            => Request<object, TResponseData>(urlRelativePath, HttpMethod.Get, null, queryParameters, headers);

        /// <summary>
        /// Requests to POST some data on an HTTP REST Service given a relative path and some optional HTTP body, query strings, headers.
        /// It also deserialize the HTTP response and returns a Result depending on the HTTP code returned.
        /// This request is to be made on our custom HTTP REST server as it only react to some HTTP code.
        /// </summary>
        /// <typeparam name="TRequestData">The type of the data include in the request body if any, object if not needed.</typeparam>
        /// <typeparam name="TResponseData">The expected type of HTTP response data received.</typeparam>
        /// <param name="urlRelativePath">The relative path of the URL. It will be concatenated with the RootUrl and also the query strings if any.</param>
        /// <param name="body">The body content to send if any. Here only JSON content are allowed for now.</param>
        /// <param name="queryParameters">The query string parameters if any. They will be appended to the absolute URL.</param>
        /// <param name="headers">The HTTP headers to set if any.</param>
        /// <returns>
        /// Success : The request has succeeded and the response was successfully deserialized into the result response type.
        /// BadPrerequisites : The RootUrl has not been set.
        /// NoConnection : Unable to connect the HTTP REST server.
        /// Unauthorized : The service requested is unauthorized or forbidden.
        /// Failure : The response deserialization has failed.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        protected virtual Task<Result<TResponseData>> Post<TRequestData, TResponseData>(string urlRelativePath, TRequestData body = default, Dictionary<string, object> queryParameters = null, Dictionary<string, string> headers = null)
            => Request<TRequestData, TResponseData>(urlRelativePath, HttpMethod.Post, body, queryParameters, headers);

        /// <summary>
        /// Requests to POST some data on an HTTP REST Service given a relative path and some optional HTTP body, query strings, headers.
        /// It also deserialize the HTTP response and returns a Result depending on the HTTP code returned.
        /// This request is to be made on our custom HTTP REST server as it only react to some HTTP code.
        /// </summary>
        /// <typeparam name="TRequestData">The type of the data include in the request body if any, object if not needed.</typeparam>
        /// <param name="urlRelativePath">The relative path of the URL. It will be concatenated with the RootUrl and also the query strings if any.</param>
        /// <param name="body">The body content to send if any. Here only JSON content are allowed for now.</param>
        /// <param name="queryParameters">The query string parameters if any. They will be appended to the absolute URL.</param>
        /// <param name="headers">The HTTP headers to set if any.</param>
        /// <returns>
        /// Success : The request has succeeded and the response was successfully deserialized into the result response type.
        /// BadPrerequisites : The RootUrl has not been set.
        /// NoConnection : Unable to connect the HTTP REST server.
        /// Unauthorized : The service requested is unauthorized or forbidden.
        /// Failure : The response deserialization has failed.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        protected virtual async Task<Result> Post<TRequestData>(string urlRelativePath, TRequestData body = default, Dictionary<string, object> queryParameters = null, Dictionary<string, string> headers = null)
            => await Request<TRequestData, object>(urlRelativePath, HttpMethod.Post, body, queryParameters, headers);

        /// <summary>
        /// Requests to POST some data on an HTTP REST Service given a relative path and some optional HTTP body, query strings, headers.
        /// It also deserialize the HTTP response and returns a Result depending on the HTTP code returned.
        /// This request is to be made on our custom HTTP REST server as it only react to some HTTP code.
        /// </summary>
        /// <param name="urlRelativePath">The relative path of the URL. It will be concatenated with the RootUrl and also the query strings if any.</param>
        /// <param name="queryParameters">The query string parameters if any. They will be appended to the absolute URL.</param>
        /// <param name="headers">The HTTP headers to set if any.</param>
        /// <returns>
        /// Success : The request has succeeded and the response was successfully deserialized into the result response type.
        /// BadPrerequisites : The RootUrl has not been set.
        /// NoConnection : Unable to connect the HTTP REST server.
        /// Unauthorized : The service requested is unauthorized or forbidden.
        /// Failure : The response deserialization has failed.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        protected virtual async Task<Result> Post(string urlRelativePath, Dictionary<string, object> queryParameters = null, Dictionary<string, string> headers = null)
            => await Request<object, object>(urlRelativePath, HttpMethod.Post, null, queryParameters, headers);

        /// <summary>
        /// Requests to PUT some data on an HTTP REST Service given a relative path and some optional HTTP body, query strings, headers.
        /// It also deserialize the HTTP response and returns a Result depending on the HTTP code returned.
        /// This request is to be made on our custom HTTP REST server as it only react to some HTTP code.
        /// </summary>
        /// <typeparam name="TRequestData">The type of the data include in the request body if any, object if not needed.</typeparam>
        /// <typeparam name="TResponseData">The expected type of HTTP response data received.</typeparam>
        /// <param name="urlRelativePath">The relative path of the URL. It will be concatenated with the RootUrl and also the query strings if any.</param>
        /// <param name="body">The body content to send if any. Here only JSON content are allowed for now.</param>
        /// <param name="queryParameters">The query string parameters if any. They will be appended to the absolute URL.</param>
        /// <param name="headers">The HTTP headers to set if any.</param>
        /// <returns>
        /// Success : The request has succeeded and the response was successfully deserialized into the result response type.
        /// BadPrerequisites : The RootUrl has not been set.
        /// NoConnection : Unable to connect the HTTP REST server.
        /// Unauthorized : The service requested is unauthorized or forbidden.
        /// Failure : The response deserialization has failed.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        protected virtual Task<Result<TResponseData>> Put<TRequestData, TResponseData>(string urlRelativePath, TRequestData body = default, Dictionary<string, object> queryParameters = null, Dictionary<string, string> headers = null)
            => Request<TRequestData, TResponseData>(urlRelativePath, HttpMethod.Put, body, queryParameters, headers);

        /// <summary>
        /// Requests to PUT some data on an HTTP REST Service given a relative path and some optional HTTP body, query strings, headers.
        /// It also deserialize the HTTP response and returns a Result depending on the HTTP code returned.
        /// This request is to be made on our custom HTTP REST server as it only react to some HTTP code.
        /// </summary>
        /// <typeparam name="TRequestData">The type of the data include in the request body if any, object if not needed.</typeparam>
        /// <param name="urlRelativePath">The relative path of the URL. It will be concatenated with the RootUrl and also the query strings if any.</param>
        /// <param name="body">The body content to send if any. Here only JSON content are allowed for now.</param>
        /// <param name="queryParameters">The query string parameters if any. They will be appended to the absolute URL.</param>
        /// <param name="headers">The HTTP headers to set if any.</param>
        /// <returns>
        /// Success : The request has succeeded and the response was successfully deserialized into the result response type.
        /// BadPrerequisites : The RootUrl has not been set.
        /// NoConnection : Unable to connect the HTTP REST server.
        /// Unauthorized : The service requested is unauthorized or forbidden.
        /// Failure : The response deserialization has failed.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        protected virtual async Task<Result> Put<TRequestData>(string urlRelativePath, TRequestData body = default, Dictionary<string, object> queryParameters = null, Dictionary<string, string> headers = null)
            => await Request<TRequestData, object>(urlRelativePath, HttpMethod.Put, body, queryParameters, headers);

        /// <summary>
        /// Requests to PUT some data on an HTTP REST Service given a relative path and some optional HTTP body, query strings, headers.
        /// It also deserialize the HTTP response and returns a Result depending on the HTTP code returned.
        /// This request is to be made on our custom HTTP REST server as it only react to some HTTP code.
        /// </summary>
        /// <param name="urlRelativePath">The relative path of the URL. It will be concatenated with the RootUrl and also the query strings if any.</param>
        /// <param name="queryParameters">The query string parameters if any. They will be appended to the absolute URL.</param>
        /// <param name="headers">The HTTP headers to set if any.</param>
        /// <returns>
        /// Success : The request has succeeded and the response was successfully deserialized into the result response type.
        /// BadPrerequisites : The RootUrl has not been set.
        /// NoConnection : Unable to connect the HTTP REST server.
        /// Unauthorized : The service requested is unauthorized or forbidden.
        /// Failure : The response deserialization has failed.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        protected virtual async Task<Result> Put(string urlRelativePath, Dictionary<string, object> queryParameters = null, Dictionary<string, string> headers = null)
            => await Request<object, object>(urlRelativePath, HttpMethod.Put, null, queryParameters, headers);

        /// <summary>
        /// Requests to DELETE some data on an HTTP REST Service given a relative path and some optional HTTP body, query strings, headers.
        /// It also deserialize the HTTP response and returns a Result depending on the HTTP code returned.
        /// This request is to be made on our custom HTTP REST server as it only react to some HTTP code.
        /// </summary>
        /// <param name="urlRelativePath">The relative path of the URL. It will be concatenated with the RootUrl and also the query strings if any.</param>
        /// <param name="queryParameters">The query string parameters if any. They will be appended to the absolute URL.</param>
        /// <param name="headers">The HTTP headers to set if any.</param>
        /// <returns>
        /// Success : The request has succeeded and the response was successfully deserialized into the result response type.
        /// BadPrerequisites : The RootUrl has not been set.
        /// NoConnection : Unable to connect the HTTP REST server.
        /// Unauthorized : The service requested is unauthorized or forbidden.
        /// Failure : The response deserialization has failed.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        protected virtual async Task<Result> Delete(string urlRelativePath, Dictionary<string, object> queryParameters = null, Dictionary<string, string> headers = null)
            => await Request<object, object>(urlRelativePath, HttpMethod.Delete, null, queryParameters, headers);

        /// <summary>
        /// Executes a request to the HTTP REST server given a relative path, a method and some optional HTTP body, query strings, headers.
        /// It also deserialize the HTTP response and returns a Result depending on the HTTP code returned.
        /// This request is to be made on our custom HTTP REST server as it only react to some HTTP code.
        /// </summary>
        /// <typeparam name="TRequestData">The type of the data include in the request body if any, object if not needed.</typeparam>
        /// <typeparam name="TResponseData">The expected type of HTTP response data received.</typeparam>
        /// <param name="urlRelativePath">The relative path of the URL. It will be concatenated with the RootUrl and also the query strings if any.</param>
        /// <param name="httpMethod">The HTTP method to call.</param>
        /// <param name="body">The body content to send if any. Here only JSON content are allowed for now.</param>
        /// <param name="queryParameters">The query string parameters if any. They will be appended to the absolute URL.</param>
        /// <param name="headers">The HTTP headers to set if any.</param>
        /// <returns>
        /// Success : The request has succeeded and the response was successfully deserialized into the result response type.
        /// BadPrerequisites : The RootUrl has not been set.
        /// NoConnection : Unable to connect the HTTP REST server.
        /// Unauthorized : The service requested is unauthorized or forbidden.
        /// Failure : The response deserialization has failed.
        /// Unexpected : An unexpected error occurs.
        /// </returns>
        protected virtual async Task<Result<TResponseData>> Request<TRequestData, TResponseData>(string urlRelativePath, HttpMethod httpMethod, TRequestData body = default, Dictionary<string, object> queryParameters = null, Dictionary<string, string> headers = null)
        {
            try
            {
                if (RootUrl.IsNullOrEmpty())
                    return Result<TResponseData>.BadPrerequisites.WithReason("The root URL should be set in the inheriting classes.");

                // UseDefaultCredentials is needed to send the Windows user info to the server
                using var httpClient = UseDefaultCredentials ? new HttpClient(new HttpClientHandler { UseDefaultCredentials = true }) : new HttpClient();
                httpClient.Timeout = Timeout;

                foreach (var (key, value) in headers ?? new Dictionary<string, string>())
                    httpClient.DefaultRequestHeaders.Add(key, value);

                var completeUri = new Uri(RootUrl).Append(urlRelativePath, queryParameters.ToQueryString());
                var message = new HttpRequestMessage(httpMethod, completeUri);

                if (body is FileDto file)
                    message.Content = new MultipartFormDataContent { { new ByteArrayContent(file.Content), "\"file\"", file.Name } };
                else if (body is string bodyString)
                    message.Content = new StringContent(bodyString, Encoding.UTF8, "text/plain");
                else if (body != null)
                {
                    bodyString = JsonConvert.SerializeObject(body, new JsonSerializerSettings { Converters = { new StringEnumConverter() } });
                    // TODO : try to replace it with System.Text.Json library when the problem of "an object property with an enum value is serialized to null" have been fixed.
                    message.Content = new StringContent(bodyString, Encoding.UTF8, "application/json");
                }

                var response = await httpClient.SendAsync(message);
                var jsonResponseBody = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var responseData = JsonSerializer.Deserialize<TResponseData>(jsonResponseBody, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        Converters = { new JsonStringEnumConverter() }
                    });
                    return new Result<TResponseData>(responseData);
                }

                ResultStatus resultStatus;
                switch (response.StatusCode)
                {
                    case HttpStatusCode.Unauthorized:
                    case HttpStatusCode.Forbidden: resultStatus = ResultStatus.Unauthorized; break;
                    default: resultStatus = ResultStatus.Unexpected; break;
                }

                return new Result<TResponseData>(resultStatus).WithReason($"An HTTP code {response.StatusCode.ToString()}({(int)response.StatusCode}) was received. BODY :{Environment.NewLine}{jsonResponseBody}");
            }
            catch (HttpRequestException exception)
            {
                return new Result<TResponseData>(exception).WithStatus(ResultStatus.NoConnection).WithReason($"An unexpected HTTP error occurs for the {httpMethod} request on the {urlRelativePath} URL.");
            }
            catch (JsonException jsonException)
            {
                return new Result<TResponseData>(jsonException).WithStatus(ResultStatus.Failure).WithReason($"The HTTP response for the {httpMethod} request on the {urlRelativePath} URL could not be deserialized.");
            }
            catch (Exception exception)
            {
                return new Result<TResponseData>(exception).WithReason($"An unexpected error occurs for the {httpMethod} request on the {urlRelativePath} URL.");
            }
        }

        /// <summary>
        /// Creates a full absolute URL given the relative path and some parameters.
        /// </summary>
        /// <param name="urlRelativePath">The URL relative path to add to the root URL.</param>
        /// <param name="queryParameters">The query parameters to add to the query string.</param>
        /// <returns>The concatenated created absolute URL.</returns>
        protected string CreateUrl(string urlRelativePath, Dictionary<string, string> queryParameters = null)
            => new Uri(RootUrl).Append(urlRelativePath, queryParameters.ToQueryString()).AbsoluteUri.TrimEnd('/');


        #endregion Methods (Virtual)

        #region Methods (Download)

        /// <summary>
        /// Downloads a raw string content from an URL.
        /// </summary>
        /// <param name="url">The URL to read the string from.</param>
        /// <returns>
        /// Success : The downloaded string is returned.
        /// BadParameters : The URL is not defined or invalid.
        /// Failure : HTTP code BadRequest (400) received.
        /// Unauthorized : HTTP code Unauthorized (401) or Forbidden(403) received.
        /// NotFound : HTTP code NotFound (404) received.
        /// Unexpected : Any other error occurs.
        /// </returns>
        protected virtual Task<Result<string>> DownloadString(string url)
            => Download<string>(url);

        /// <summary>
        /// Downloads a raw binary content from an URL.
        /// </summary>
        /// <param name="url">The URL to read the binary from.</param>
        /// <returns>
        /// Success : The downloaded binary is returned.
        /// BadParameters : The URL is not defined or invalid.
        /// Failure : HTTP code BadRequest (400) received.
        /// Unauthorized : HTTP code Unauthorized (401) or Forbidden(403) received.
        /// NotFound : HTTP code NotFound (404) received.
        /// Unexpected : Any other error occurs.
        /// </returns>
        protected virtual Task<Result<byte[]>> DownloadBytes(string url)
            => Download<byte[]>(url);

        /// <summary>
        /// Downloads a raw binary or a string content from an URL.
        /// </summary>
        /// <param name="url">The URL to read the string from.</param>
        /// <returns>
        /// Success : The downloaded binary is returned.
        /// BadParameters : The URL is not defined or invalid.
        /// Failure : HTTP code BadRequest (400) received.
        /// Unauthorized : HTTP code Unauthorized (401) or Forbidden(403) received.
        /// NotFound : HTTP code NotFound (404) received.
        /// Unexpected : Any other error occurs.
        /// </returns>
        private async Task<Result<TContent>> Download<TContent>(string url)
        {
            try
            {
                if (url.IsNullOrEmpty() || !url.StartsWith("http"))
                    return Result<TContent>.BadParameters.WithReason($"The URL {url} is not a valid URL.");

                using var httpClient = UseDefaultCredentials ? new HttpClient(new HttpClientHandler { UseDefaultCredentials = true }) : new HttpClient();

                var data = typeof(TContent) == typeof(string)
                    ? (object)await httpClient.GetStringAsync(url)
                    : await httpClient.GetByteArrayAsync(url);

                return new Result<TContent>((TContent)data);
            }
            catch (WebException exception)
            {
                Console.WriteLine($"DOWNLOAD RESULT: {exception.ToDetailedString()}");
                if (!(exception.Response is HttpWebResponse httpWebResponse))
                    return new Result<TContent>(exception).WithReason("The response should be an HTTP response");

                return httpWebResponse.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => Result<TContent>.Unauthorized.WithException(exception),
                    HttpStatusCode.Forbidden => Result<TContent>.Unauthorized.WithException(exception),
                    HttpStatusCode.BadRequest => Result<TContent>.Failure.WithException(exception),
                    HttpStatusCode.NotFound => Result<TContent>.NotFound.WithException(exception),
                    _ => new Result<TContent>(exception)
                };
            }
            catch (Exception exception)
            {
                return new Result<TContent>(exception);
            }
        }

        #endregion Methods (Download)

        #region Methods (Abstract)

        /// <summary>
        /// The root URL to call.
        /// This normally should be set in a configuration file.
        /// </summary>
        protected abstract string RootUrl { get; }

        /// <summary>
        /// Whether the HTTP client should use the default user credentials.
        /// </summary>
        protected abstract bool UseDefaultCredentials { get; }

        #endregion Methods (Abstract)
    }
}