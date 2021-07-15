using Autofac;
using GifToBase64Extractor.Dependency_Injection;
using System;

namespace GifToBase64Extractor
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = ContainerConfig.Configure();

            // create a instance of a application
            using (var scope = container.BeginLifetimeScope())
            {
                // Autofac Nuget package
                var app = scope.Resolve<IApplication>();
                app.Run();
            }

            Console.ReadLine();
        }
    }
}
