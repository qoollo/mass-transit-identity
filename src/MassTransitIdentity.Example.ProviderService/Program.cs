using MassTransit;
using MassTransitIdentity.Core;
using MassTransitIdentity.Example.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MassTransitIdentity.Example.ProviderService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<TestIdentityPublisher>();

                    services.AddScoped<MassTransitIdentityToken>();

                    services.AddMassTransit(x =>
                    {
                        x.AddRequestClient<IdentityExampleRequest>();

                        x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.UsePublishFilter(typeof(IdentityTokenPublishFilter<>), context);
                            cfg.UseConsumeFilter(typeof(IdentityTokenConsumeFilter<>), context);

                            cfg.ConfigureEndpoints(context);

                            cfg.Host("localhost", "/", h =>
                            {
                                h.Username("guest");
                                h.Password("guest");
                            });
                        });
                    });
                });
    }
}
