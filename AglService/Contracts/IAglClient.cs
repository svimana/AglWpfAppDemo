namespace AglService.Contracts
{
    using System.Threading.Tasks;
    using RestSharp;

    /// <summary>
    /// The IAglClient interface.
    /// </summary>
    public interface IAglClient
    {
        /// <summary>
        /// The get request for restRequest async.
        /// </summary>
        /// <param name="restRequest">The restRequest.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<IRestResponse> PerformRequestForPathAsync(IRestRequest restRequest);
    }
}
