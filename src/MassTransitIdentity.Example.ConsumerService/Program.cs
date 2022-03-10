using MassTransit;
using MassTransitIdentity.Core;
using MassTransitIdentity.Example.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MassTransitIdentity.Example.ConsumerService
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
                        x.AddRequestClient<AnotherIdentityExampleRequest>();

                        x.AddConsumer<TestIdentityConsumer>()
                            .Endpoint(cfg =>
                            {
                                
                                cfg.Name = "identity-test";
                            });
                        
                        x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.UsePublishFilter(typeof(IdentityTokenPublishFilter<>), context);
                            cfg.UseSendFilter(typeof(IdentityTokenSendFilter<>), context);
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
