using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Core;

namespace RhythmCodex.Ioc
{
    public class Resolver : IResolver
    {
        private readonly Lazy<IContainer> _container;
        
        public Resolver()
        {
            _container = new Lazy<IContainer>(BuildContainer);
        }

        private IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyModules(GetType().Assembly);
            return builder.Build();
        }

        private T ResolveUnregistered<T>(IEnumerable<Parameter> parameters)
            where T : class
        {
            var serviceType = typeof(T);
            var scope = _container.Value.Resolve<ILifetimeScope>();
            using (var innerScope = scope.BeginLifetimeScope(b => b.RegisterType(serviceType)))
            {
                IComponentRegistration reg;
                innerScope.ComponentRegistry.TryGetRegistration(new TypedService(serviceType), out reg);

                return _container.Value.ResolveComponent(reg, parameters) as T;
            }
        }

        public T Resolve<T>()
            where T : class
        {
            if (_container.Value.IsRegistered(typeof(T)))
                return _container.Value.Resolve<T>();

            return ResolveUnregistered<T>(Enumerable.Empty<Parameter>());
        }
    }
}
