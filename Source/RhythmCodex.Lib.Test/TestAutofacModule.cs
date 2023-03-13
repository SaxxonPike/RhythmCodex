using Autofac;
using RhythmCodex.IoC;

namespace RhythmCodex
{
    public class TestAutofacModule : Autofac.Module
    {
        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            var mappings = ServiceTypes.GetMappings();

            foreach (var mapping in mappings)
            {
                var registration = builder.RegisterType(mapping.Implementation);
                foreach (var service in mapping.Services)
                    registration.As(service)
                        .AsSelf();
                if (mapping.SingleInstance)
                    registration.SingleInstance();
                else
                    registration.InstancePerDependency();
            }
        }
    }
}