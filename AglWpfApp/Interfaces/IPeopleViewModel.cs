namespace AglWpfApp.Interfaces
{
    using AglDto;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;

    public interface IPeopleViewModel
    {
        Task Initialize();

        IList<string> AllMaleOwnerCats { get; }

        IList<string> AllFemaleOwnerCats { get; }
    }
}
