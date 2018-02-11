namespace AglWpfApp.ViewModels
{
    using AglWpfApp.Interfaces;
    using AglWpfApp.Properties;
    using Caliburn.Micro;
    using Serilog;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    public class ShellViewModel : Screen, IShellViewModel
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly ILogger Logger;

        public ShellViewModel(IPeopleViewModel peopleViewModel, ILogger logger)
        {
            this.Logger = logger;
            this.PeopleViewModel = peopleViewModel;
        }

        
        /// <summary>
        /// Gets a values of the people view model.
        /// </summary>
        public IPeopleViewModel PeopleViewModel { get; private set; }

        /// <summary>
        /// The method to refresh the list of owners on the UI
        /// </summary>
        public void RefreshOwners()
        {
            this.PeopleViewModel.Initialize();
        }

        /// <summary>
        /// The on view loaded
        /// </summary>
        /// <param name="view">The view</param>
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            this.DisplayName = $"{Resources.ShellViewTitle}  v{typeof(ShellViewModel).Assembly.GetName().Version.ToString(3)}";
            this.Logger.Information($"Main Shell View loaded.");
            this.Logger.Information($"{this.DisplayName}");

            try
            {
                Task.Run(
                  async  () =>
                    {
                        this.Logger.Information($"Initialize People View Model");
                        if (this.PeopleViewModel != null)
                        {
                            await PeopleViewModel.Initialize();
                        }
                        else
                        {
                            this.Logger.Warning($"Could not initialize People View Model.");
                        }
                    });
            }
            catch (Exception ex)
            {
                this.Logger.Error($"Could not Initialize PeopleViewModel. Exception error {ex.Message}");
            }
        }

    }
}
