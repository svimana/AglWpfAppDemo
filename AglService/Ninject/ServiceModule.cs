using AglService.Contracts;
using Ninject.Modules;

namespace AglService.Ninject
{
    /// <summary>
    /// The Service Module class.
    /// </summary>
    public class ServiceModule : NinjectModule
    {
        /// <summary>
        /// The load method
        /// </summary>
        public override void Load()
        {
            this.Bind<IAglClient>().To<AglWebApiClient>().InSingletonScope();
            this.Bind<IAglPeopleService>().To<AglPeopleService>().InSingletonScope();
        }
    }
}
