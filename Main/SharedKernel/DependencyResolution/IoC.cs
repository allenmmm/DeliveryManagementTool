using StructureMap;
using System;

namespace DMT.SharedKernel.DependencyResolution
{
    public static class IoC
    {
        private static IContainer _container = BuildDefaultContainer();
        public static IContainer Container { get { return _container; } }

        public static void AddRegistry<T>() where T : Registry
        {
            Registry reg = (T)Activator.CreateInstance(typeof(T));

            _container.Configure(c =>
            {
                c.AddRegistry(reg);
            });
        }

        public static void ClearRegistries()
        {
            _container = BuildDefaultContainer();
        }

        private static IContainer BuildDefaultContainer()
        {
            return new Container(c => c.AddRegistry<DefaultRegistry>());
        }
    }
}
