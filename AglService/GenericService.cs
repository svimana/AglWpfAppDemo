namespace AglService
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;

    using AglService.Contracts;
    using Newtonsoft.Json;
    using RestSharp;
    using Serilog;

    /// <summary>
    /// The generic service.
    /// </summary>
    public abstract class GenericService
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly ILogger Logger;

        /// <summary>
        /// The agl client.
        /// </summary>
        private readonly IAglClient aglClient;

        /// <summary>
        /// Initialises a new instance of the <see cref="GenericService"/> class.
        /// </summary>
        /// <param name="aglClient">The agl client.</param>
        /// <param name="logger">The logger.</param>
        protected GenericService(IAglClient aglClient, ILogger logger)
        {
            this.aglClient = aglClient;
            this.Logger = logger;
        }

        /// <summary>
        /// Gets the base end point for use with the rest request.
        /// </summary>
        protected abstract string BaseEndPoint { get; }

        /// <summary>
        /// Generate a rest request for a given resources with optional parameters
        /// </summary>
        /// <param name="resource">The resource endpoint.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A Rest request</returns>
        protected static RestRequest GenerateRestRequestWithParams(string resource, IDictionary<string, object> parameters)
        {
            var restRequest = new RestRequest { Method = Method.GET, Resource = resource };
            foreach (var param in parameters)
            {
                restRequest.AddParameter(param.Key, param.Value);
            }

            return restRequest;
        }

        /// <summary>
        /// The get request with resource.
        /// </summary>
        /// <param name="restRequest">The rest request.</param>
        /// <typeparam name="T">The type to marshall the JSON to</typeparam>
        /// <returns>The <see cref="Task"/> with the marshalled object.</returns>
        protected async Task<T> GetRequestWithPath<T>(RestRequest restRequest)
        {
            return await this.PerformGenericRequest<T>(restRequest, Method.GET);
        }

        /// <summary>
        /// The get request with resource.
        /// </summary>
        /// <typeparam name="T">The type to marshall the JSON to.</typeparam>
        /// <param name="restRequest">The rest request.</param>
        /// <returns>The <see cref="Task"/> with the marshalled object.</returns>
        protected async Task<T> PutRequestWithQuery<T>(RestRequest restRequest)
        {
            return await this.PerformGenericRequest<T>(restRequest, Method.PUT);
        }

        /// <summary>
        /// The post request with resource.
        /// </summary>
        /// <typeparam name="T">The type to marshall the JSON to.</typeparam>
        /// <param name="restRequest">The rest request.</param>
        /// <returns>The <see cref="Task"/> with the marshalled object.</returns>
        protected async Task<T> PostRequestWithQuery<T>(RestRequest restRequest)
        {
            return await this.PerformGenericRequest<T>(restRequest, Method.POST);
        }

        /// <summary>
        /// Get the request with path.
        /// </summary>
        /// <typeparam name="T">The type to marshall the JSON to.</typeparam>
        /// <param name="subPath">The sub path.</param>
        /// <returns>The <see cref="Task"/> with the marshalled object.</returns>
        protected async Task<T> GetRequestWithPath<T>(string subPath)
        {
            var restRequest = new RestRequest { Resource = this.BaseEndPoint + subPath };
            return await this.GetRequestWithPath<T>(restRequest);
        }

        /// <summary>
        /// Get the request with path.
        /// </summary>
        /// <typeparam name="T">The type to marshall the JSON to.</typeparam>
        /// <param name="query">The sub path.</param>
        /// <param name="requestBody">The request body.</param>
        /// <returns>The <see cref="Task"/> with the marshalled object.</returns>
        protected async Task<T> PutRequestWithQuery<T>(string query, object requestBody = null)
        {
            var restRequest = new RestRequest { Resource = this.BaseEndPoint + query };
            restRequest.AddHeader("Content-type", "application/json");
            if (requestBody != null)
            {
                restRequest.AddJsonBody(requestBody);
            }

            return await this.PutRequestWithQuery<T>(restRequest);
        }

        /// <summary>
        /// Get the request with path.
        /// </summary>
        /// <typeparam name="T">The type to marshall the JSON to.</typeparam>
        /// <param name="query">The sub path.</param>
        /// <param name="requestBody">The request body.</param>
        /// <returns>The <see cref="Task"/> with the marshalled object.</returns>
        protected async Task<T> PostRequestWithQuery<T>(string query, object requestBody = null)
        {
            var restRequest = new RestRequest { Resource = this.BaseEndPoint + query };
            restRequest.AddHeader("Content-type", "application/json");
            if (requestBody != null)
            {
                restRequest.AddJsonBody(requestBody);
            }

            return await this.PostRequestWithQuery<T>(restRequest);
        }

        /// <summary>
        /// Perform a web request for a method.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the response to.</typeparam>
        /// <param name="restRequest">The rest request.</param>
        /// <param name="requestMethod">The request method.</param>
        /// <returns>The <see cref="Task"/> containing the deserialized object or default if deserialized with errors.</returns>
        private async Task<T> PerformGenericRequest<T>(RestRequest restRequest, Method requestMethod)
        {
            try
            {
                restRequest.Method = requestMethod;
                restRequest.AddHeader("KeepAlive", "false");
                restRequest.AddHeader("ProtocolVersion", "HttpVersion.Version10");
                restRequest.AddHeader("Content-Length", "65535");
                this.Logger.Information($"Sending {requestMethod}\t{restRequest.Resource} request to AGL web service");
                var response = await this.aglClient.PerformRequestForPathAsync(restRequest);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    if (response.ErrorException != null)
                    {
                        this.Logger.Error(response.ErrorException, response.ErrorMessage);
                    }
                    else
                    {
                        this.Logger.Error(
                            new Exception(response.ErrorMessage),
                            $"AGL web service request failed. {response.ErrorMessage}");
                    }

                    return default(T);
                }

                this.Logger.Information("Success.");
                return !string.IsNullOrEmpty(response.Content) ? JsonConvert.DeserializeObject<T>(response.Content) : default(T);
            }
            catch (Exception ex) when (ex is JsonException || ex is TaskCanceledException)
            {
                this.Logger.Error(ex.Message);
                return default(T);
            }
        }
    }

}
