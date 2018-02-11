namespace AglWpfApp.ViewModels
{
    using AglDto;
    using AglService;
    using AglService.Contracts;
    using AglWpfApp.Interfaces;
    using Caliburn.Micro;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class PeopleViewModel : PropertyChangedBase, IPeopleViewModel
    {

        /// <summary>
        /// The people service.
        /// </summary>
        private readonly IAglPeopleService aglPeopleService;

        public PeopleViewModel(IAglPeopleService aglPeopleService)
        {
            this.aglPeopleService = aglPeopleService;
        }

        public IList<Person> People { get; private set; }

        public IList<string> AllFemaleOwnerCats
        {
            get
            {
                if (this.People == null)
                {
                    return null;
                }

                var femaleOwnerPets = this.People?.Where(p => p.IsFemale && p.Pets != null).Select(p => p.Pets).ToList();

                var pets = femaleOwnerPets.Aggregate((p1,p2) => p1.Union(p2).ToList());

                return pets.Where(c => c.IsCat != null && (bool)c.IsCat).Select(p => p.Name).ToList();
            }
        }

        public IList<string> AllMaleOwnerCats
        {
            get
            {
                if (this.People == null)
                {
                    return null;
                }

                var maleOwnerPets = this.People?.Where(p => p.IsMale && p.Pets != null).Select(p => p.Pets).ToList();

                var pets = maleOwnerPets.Aggregate((p1, p2) => p1.Union(p2).ToList());

                return pets.Where(c => c.IsCat != null && (bool)c.IsCat).Select(p => p.Name).ToList();
            }
        }

        public async Task Initialize()
        {
            var people = await this.GetAllPeople();

            this.People = people.ToList();
            this.NotifyOfPropertyChange(() => this.AllFemaleOwnerCats);
            this.NotifyOfPropertyChange(() => this.AllMaleOwnerCats);
        }

        protected async Task<IEnumerable<Person>> GetAllPeople()
        {
            var people = await this.aglPeopleService.GetAll();

            return people;
        }
    }
}
