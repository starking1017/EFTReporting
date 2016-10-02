/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:EFTReporting"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using EFTReporting.ViewModels;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace EFTReporting.ViewModels
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<AdminViewModel>();
            SimpleIoc.Default.Register<PaymentViewModel>();
        }

        public MainViewModel MainViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }
        public AdminViewModel AdminViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AdminViewModel>();
            }
        }
        public PaymentViewModel PaymentViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<PaymentViewModel>();
            }
        }
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
            if (SimpleIoc.Default.IsRegistered<MainViewModel>())
                SimpleIoc.Default.Unregister<MainViewModel>();

            if (SimpleIoc.Default.IsRegistered<AdminViewModel>())
                SimpleIoc.Default.Unregister<AdminViewModel>();

            if (SimpleIoc.Default.IsRegistered<PaymentViewModel>())
                SimpleIoc.Default.Unregister<PaymentViewModel>();

        }
    }
}