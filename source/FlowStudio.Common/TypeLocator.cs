namespace FlowStudio.Common
{
    using System;
    using System.Configuration;
    using System.Globalization;
    using Microsoft.Practices.Unity;
    using Microsoft.Practices.Unity.Configuration;

    public static class TypeLocator
    {
        public static IUnityContainer Container { get; private set; }

        static TypeLocator()
        {
            var unityContainer = new UnityContainer();
            var section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
            if (section != null)
            {
                section.Configure(unityContainer);
            }
            Container = unityContainer;
        }

        public static T GetInstance<T>()
        {
            if (Container.IsRegistered(typeof(T)))
            {
                return Container.Resolve<T>();
            }

            throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "No registration found for type [{0}]", typeof(T)));
        }
    }
}
