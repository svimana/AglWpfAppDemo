namespace AglService.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using AglDto;

    /// <summary>
    /// The IAglPeopleService interface.
    /// </summary>
    public interface IAglPeopleService
    {
        /// <summary>
        /// The get all.
        /// </summary>
        /// <returns>The <see cref="Task" />.</returns>
        Task<IEnumerable<Person>> GetAll();
    }
}
