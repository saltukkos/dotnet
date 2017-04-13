using System.Windows;
using Caliburn.Micro;
using Saltuk.Nsudotnet.EnigmaWPFWrapper.ViewModels;

namespace Saltuk.Nsudotnet.EnigmaWPFWrapper
{
    class AppBootstrapper : BootstrapperBase
    {
        public AppBootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs startupEventArgs)
        {
            DisplayRootViewFor<InputViewModel>();
        }

    }
}
