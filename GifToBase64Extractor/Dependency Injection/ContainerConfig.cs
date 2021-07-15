using Autofac;
using MediaToBase64ExtractorCore.Utilities;
using System.Linq;
using System.Reflection;

namespace GifToBase64Extractor.Dependency_Injection
{
    public static class ContainerConfig
    {
        public static IContainer Configure()
        {
            var builder = new ContainerBuilder();

            //builder.RegisterType<Application>(); // create a new Instance
            builder.RegisterType<Application>().As<IApplication>();

            // response with one Instance when i need a BuisnessLogic Item
            builder.RegisterType<GifToBase64Extractor.Modules.DragonRush_ImageExtractor>().As<IMediaExtractor>();

            builder.RegisterAssemblyTypes(Assembly.Load(nameof(MediaToBase64ExtractorCore)))
                .Where(t => t.Namespace.Contains("Utilities")) // opt. || klause
                .As(t => t.GetInterfaces().FirstOrDefault(i => i.Name == "I" + t.Name));

            return builder.Build();
        }
    }
}
