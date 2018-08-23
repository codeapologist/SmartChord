using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Practices.ServiceLocation;
using Prism.Events;
using Prism.Modularity;
using Prism.Regions;
using Prism.StructureMap;

namespace SmartChord.DesktopClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var bootstrapper = new Bootstrapper();
            bootstrapper.Run();
        }
    }

    public class Bootstrapper : StructureMapBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.GetInstance<MainWindow>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow = Shell as MainWindow;
            Application.Current.MainWindow.Show();
        }

        //protected override IModuleCatalog CreateModuleCatalog()
        //{
        //    return new ConfigurationModuleCatalog();
        //}
        //protected virtual void ConfigureContainer()
        //{
        //    this.Container.Configure(x => x.For<IServiceLocator>().Use<StructureMapServiceLocatorAdapter>().Singleton());
        //    this.Container.Configure(x => x.For<IModuleInitializer>().Use<ModuleInitializer>());
        //    this.Container.Configure(x => x.For<RegionAdapterMappings>().Use<RegionAdapterMappings>());
        //    this.Container.Configure(x => x.For<IRegionManager>().Use<RegionManager>());
        //    this.Container.Configure(x => x.For<IEventAggregator>().Use<EventAggregator>());
        //    this.Container.Configure(x => x.For<IRegionViewRegistry>().Use<RegionViewRegistry>());
        //    this.Container.Configure(x => x.For<IRegionBehaviorFactory>().Use<RegionBehaviorFactory>());
        //    this.Container.Configure(x => x.For<IRegionNavigationJournalEntry>().Use<RegionNavigationJournalEntry>());
        //    this.Container.Configure(x => x.For<IRegionNavigationJournal>().Use<RegionNavigationJournal>());
        //    this.Container.Configure(x => x.For<IRegionNavigationService>().Use<RegionNavigationService>());
        //    this.Container.Configure(x => x.For<IRegionNavigationContentLoader>().Use<RegionNavigationContentLoader>());


        //}
    }

}
