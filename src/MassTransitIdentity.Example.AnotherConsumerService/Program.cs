using MassTransit;
using MassTransitIdentity.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MassTransitIdentity.Example.AnotherConsumerService
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
                    services.AddScoped<MassTransitIdentityToken>();
                    
                    services.AddMassTransit(x =>
                    {
                        x.AddConsumer<AnotherTestIdentityConsumer>()
                            .Endpoint(cfg =>
                            {

                                cfg.Name = "another-identity-test";
                            });
                        x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.UseConsumeFilter(typeof(IdentityTokenConsumeFilter<>), context);
                            cfg.UseSendFilter(typeof(IdentityTokenSendFilter<>), context);

                            cfg.ConfigureEndpoints(context);

                            cfg.Host("localhost", "/", h =>
                            {
                                h.Username("guest");
                                h.Password("guest");
                            });
                        });
                    }) ;
                });
    }
}
