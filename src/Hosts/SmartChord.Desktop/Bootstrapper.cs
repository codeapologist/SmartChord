using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using MediatR;
using StructureMap;
using Thinktecture;
using Thinktecture.Adapters;

namespace SmartChord.Desktop
{
    public class Bootstrapper : BootstrapperBase
    {
        private IContainer _container;

        public Bootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<MainWindowViewModel>();
            
        }


        protected override void Configure()
        {
            
            _container =  new Container(cfg =>
            {
                cfg.Scan(scanner =>
                {
                    scanner.AssembliesFromApplicationBaseDirectory(assem => assem.FullName.StartsWith("SmartChord."));
                    scanner.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<>)); // Handlers with no response
                    scanner.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>)); // Handlers with a response
                    scanner.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
                });
                cfg.For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => ctx.GetInstance);
                cfg.For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => ctx.GetAllInstances);
                cfg.For<IMediator>().Use<Mediator>();
                cfg.For<IWindowManager>().Use<WindowManager>();
                cfg.For<IEnvironment>().Use<EnvironmentAdapter>();
                cfg.For<IDialogService>().Use<DialogService>();
            });
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service).Cast<object>();
        }

        protected override void BuildUp(object instance)
        {

        }
    }
}