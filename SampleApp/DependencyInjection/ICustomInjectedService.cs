using Microsoft.Extensions.DependencyInjection;
using System;

namespace SampleApp.DependencyInjection
{
    public interface ICustomInjectedService
    {
        void Execute(string input);
        void PrintServices();
    }

    public class CustomInjectedService : ICustomInjectedService
    {
        private readonly IServiceCollection services;

        public CustomInjectedService(IServiceCollection services)
        {
            this.services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public void Execute(string input)
        {
            Console.WriteLine($"Injected Service: {input}");
        }

        public void PrintServices()
        {
            Console.WriteLine("Injeced Service: All available services\n");

            foreach (var service in services)
            {
                Console.WriteLine($" -> {service.ServiceType.Name } implemented by {service.ImplementationType?.Name ?? service.ImplementationInstance?.GetType().Name ?? service.ImplementationFactory.ToString()} with lifetime of {service.Lifetime}");
            }

            Console.WriteLine("\n");
        }
    }
}
