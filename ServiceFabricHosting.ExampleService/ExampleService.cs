using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.ServiceFabric.Services.Runtime;

namespace ServiceFabricHosting.ExampleService
{
    internal sealed class ExampleService : StatelessService
    {
        public ExampleService(StatelessServiceContext context) : base(context)
        {
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            var hostBuilder = new HostBuilder()
                .ConfigureServices(services => services
                    .AddHostedService<ExampleBackgroundService>());

            using (var host = hostBuilder.Build())
            {
                await host.RunAsync(cancellationToken);
            }

            ServiceEventSource.Current.Message("RunAsync() COMPLETED");
        }
    }

    internal class ExampleBackgroundService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            long iterations = 0;

            while (true)
            {
                stoppingToken.ThrowIfCancellationRequested();

                ServiceEventSource.Current.Message("Working-{0}", ++iterations);

                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }
    }
}
