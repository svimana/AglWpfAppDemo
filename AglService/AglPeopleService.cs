namespace AglService
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AglDto;
    using Contracts;
    using Serilog;

    public class AglPeopleService : GenericService, IAglPeopleService
    {
        protected override string BaseEndPoint => @"/people.json";

        public AglPeopleService(IAglClient aglClient, ILogger logger) : base(aglClient, logger)
        {

        }

        public async Task<IEnumerable<Person>> GetAll()
        {
            var restRequest = GenerateRestRequestWithParams(this.BaseEndPoint, new Dictionary<string, object>());
            var people = await this.GetRequestWithPath<IEnumerable<Person>>(restRequest);
            return people;
        }
    }
}
