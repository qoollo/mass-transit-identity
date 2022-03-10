using MassTransit;
using MassTransitIdentity.Core;
using MassTransitIdentity.Example.Contracts;
using MassTransitIdentity.Example.ProviderService.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MassTransitIdentity.Example.ProviderService
{
    public class TestIdentityPublisher : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TestIdentityPublisher> _logger;
        private readonly IConfiguration _configuration;
        private readonly Random _random;

        private Timer? _timer;

        public TestIdentityPublisher(IServiceProvider serviceProvider,
            ILogger<TestIdentityPublisher> logger,
            IConfiguration configuration)
        { 
            _random = new Random();

            _serviceProvider = serviceProvider;
            _logger = logger;
            _configuration = configuration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var publishConfiguration = new PublishConfiguration();
            _configuration.GetSection("Publish").Bind(publishConfiguration);

            _timer = new Timer(GetMyIdentity, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(publishConfiguration.Timeout));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private void GetMyIdentity(object? state)
        {
            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                Guid clientId = Guid.NewGuid();
                var token = (MassTransitIdentityToken)scope.ServiceProvider.GetService(typeof(MassTransitIdentityToken))!;

                var publishConfiguration = new PublishConfiguration();
                _configuration.GetSection("Publish").Bind(publishConfiguration);
                if (_random.Next(101) >= publishConfiguration.NullChance)
                {
                    token.Value = clientId;
                }
                
                var requestClient = scope.ServiceProvider.GetService<IRequestClient<IdentityExampleRequest>>()!;
                var response = requestClient.GetResponse<IdentityExampleResponse>(new { });

                _logger.LogInformation($"Returned identity token {response.Result.Message.Token} to client with id {clientId}");
            }
        }
    }
}
