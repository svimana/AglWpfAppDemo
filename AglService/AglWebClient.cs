namespace AglService
{
    using System.Configuration;
    using System.Threading.Tasks;
    using RestSharp;
    using Contracts;

    /// <summary>
    /// The agl web api client.
    /// </summary>
    public class AglWebApiClient : IAglClient
    {
        /// <summary>
        /// The host name.
        /// </summary>
        private static readonly string BaseUrl;

        /// <summary>
        /// The timeout.
        /// </summary>
        private static readonly int ConfiguredTimeout;

        /// <summary>
        /// The http client.
        /// </summary>
        private readonly RestClient restClient;

        /// <summary>
        /// Initialises static members of the <see cref="AglWebApiClient"/> class
        /// </summary>
        static AglWebApiClient()
        {
            var configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            BaseUrl = configuration.AppSettings.Settings["AglBaseUrl"].Value;

            if (int.TryParse(configuration.AppSettings.Settings["Timeout"].Value, out var timeout))
            {
                ConfiguredTimeout = timeout;
            }
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="AglWebApiClient" /> class.
        /// </summary>
        public AglWebApiClient()
        {
            this.restClient = new RestClient(BaseUrl) { Timeout = ConfiguredTimeout };
        }

        /// <summary>
        /// The get request for resource async.
        /// </summary>
        /// <param name="restRequest">The rest request.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<IRestResponse> PerformRequestForPathAsync(IRestRequest restRequest)
        {
            var response = await this.restClient.ExecuteTaskAsync(restRequest);
            return response;
        }
    }
}
