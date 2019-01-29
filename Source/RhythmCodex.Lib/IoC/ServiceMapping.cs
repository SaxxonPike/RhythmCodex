using System;
using System.Collections.Generic;
using System.Linq;

namespace RhythmCodex.IoC
{
    /// <summary>
    /// A type map for a single service.
    /// </summary>
    public struct ServiceMapping
    {
        internal ServiceMapping(Type implementation, IEnumerable<Type> services, bool singleInstance)
        {
            Services = services.ToArray();
            Implementation = implementation;
            SingleInstance = singleInstance;

            if (Services.Any(s => !s.IsInterface))
                throw new Exception($"Services must all be interfaces. (Implementation: {Implementation})");
        }

        /// <summary>
        /// Implementation of a service.
        /// </summary>
        public Type Implementation { get; }

        /// <summary>
        /// Interfaces that a service should implement. These should all be interfaces.
        /// </summary>
        public Type[] Services { get; }
        
        /// <summary>
        /// If true, this service should only have a single instance in the container.
        /// </summary>
        public bool SingleInstance { get; }
    }
}