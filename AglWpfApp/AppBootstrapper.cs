namespace AglWpfApp
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Windows;

    using AglWpfApp.ViewModels;
    using AglWpfApp.Interfaces;
    using Caliburn.Micro;
    using Microsoft.Extensions.Logging;
    using Ninject;
    using Serilog;

    using ILogger = Serilog.ILogger;
    using Serilog.Events;
    using AglService.Ninject;

    /// <summary>
    /// The application bootstrapper.
    /// </summary>
    public sealed class AppBootstrapper : BootstrapperBase, IDisposable
    {
        /// <summary>
        /// The logger factory.
        /// </summary>
        private static readonly ILoggerFactory LoggerFactory = new LoggerFactory();

        /// <summary>
        /// The kernel.
        /// </summary>
        private IKernel kernel;

        /// <summary>Initialises a new instance of the <see cref="AppBootstrapper"/> class. 
        /// Initialises a new instance of the <see cref="AppBootstrapper"/> class.</summary>
        public AppBootstrapper()
        {
            SetupLogging();
            this.Initialize();
        }

        /// <summary>
        /// Disposal of resources
        /// </summary>
        public void Dispose()
        {
            ////TODO Put cleanup logic here
        }

        /// <summary>
        /// The configure.
        /// </summary>
        protected override void Configure()
        {
            var logger = Log.ForContext<AppBootstrapper>();
            logger.Debug("Configuring Appbootstrapper.");
            this.InitialiseKernel();
            this.BindWpfDependencies();
        }

        /// <summary>
        /// The get all instances.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <returns>All instances for the specified type</returns>
        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            try
            {
                return this.kernel.GetAll(serviceType);
            }
            catch (ActivationException activationException)
            {
                var errorMessage = $"Could not locate any instances of the type {serviceType.Name}";
                Log.Error(errorMessage);
                throw new InvalidOperationException(errorMessage, activationException);
            }
        }

        /// <summary>
        /// The get instance.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <param name="key">The key.</param>
        /// <returns>The <see cref="object"/>.</returns>
        /// <exception cref="InvalidOperationException">If an instance of the contract could not be located.</exception>
        protected override object GetInstance(Type serviceType, string key)
        {
            try
            {
                return string.IsNullOrEmpty(key) ? this.kernel.Get(serviceType) : this.kernel.Get(serviceType, key);
            }
            catch (ActivationException activationException)
            {
                var errorMessage = string.IsNullOrEmpty(key)
                                       ? $"Could not locate any instances of the type {serviceType.Name}."
                                       : $"Could not locate any instances of the type {serviceType.Name}, with the contract {key}.";
                Log.Error(errorMessage);
                throw new InvalidOperationException(errorMessage, activationException);
            }
        }

        /// <summary>
        /// Startup sequence
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="startupEventArgs">The startup event args.</param>
        protected override void OnStartup(object sender, StartupEventArgs startupEventArgs)
        {
            this.DisplayRootViewFor<IShellViewModel>();
        }

        /// <summary>
        /// Setup logging
        /// </summary>
        private static void SetupLogging()
        {
            var applicationData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var assembly = typeof(AppBootstrapper).Assembly;

            // create the company folder
            var assemblyCompanyAttribute = assembly.GetCustomAttribute<System.Reflection.AssemblyCompanyAttribute>();
            var companyApplicationData = applicationData;
            if (assemblyCompanyAttribute != null)
            {
                companyApplicationData = Path.Combine(applicationData, assemblyCompanyAttribute.Company);
                if (!Directory.Exists(companyApplicationData))
                {
                    Directory.CreateDirectory(companyApplicationData);
                }
            }

            // create the product folder
            var assemblyProductAttribute = assembly.GetCustomAttribute<AssemblyProductAttribute>();
            var productApplicationData = companyApplicationData;
            if (assemblyProductAttribute != null)
            {
                productApplicationData = Path.Combine(companyApplicationData, assemblyProductAttribute.Product);
                if (!Directory.Exists(productApplicationData))
                {
                    Directory.CreateDirectory(productApplicationData);
                }
            }

            Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo
                .RollingFile(Path.Combine(productApplicationData, "log-{Date}.json"), LogEventLevel.Information).CreateLogger();

            //LogManager.GetLog = type => Caliburn.Micro.LogManager.GetLog(type);
        }

        /// <summary>
        /// Ninject bindings
        /// </summary>
        private void BindWpfDependencies()
        {
            this.kernel.Bind<IPeopleViewModel>().To<PeopleViewModel>().InSingletonScope();
            this.kernel.Bind<IWindowManager>().ToConstant(new WindowManager());
            this.kernel.Bind<IShellViewModel>().To<ShellViewModel>().InSingletonScope();
            this.kernel.Bind<ILogger>().ToConstant(Log.Logger).InSingletonScope();
            this.kernel.Bind<ILoggerFactory>().ToConstant(LoggerFactory).InSingletonScope();
        }

        /// <summary>
        /// Initialise the Kernel.
        /// </summary>
        private void InitialiseKernel()
        {
            this.kernel = new StandardKernel();
            this.kernel.Load(new ServiceModule());
        }
    }
}
