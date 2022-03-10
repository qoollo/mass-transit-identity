using MassTransit;
using MassTransitIdentity.Core;
using MassTransitIdentity.Example.Contracts;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace MassTransitIdentity.Example.AnotherConsumerService
{
    public class AnotherTestIdentityConsumer : IConsumer<AnotherIdentityExampleRequest>
    {
        private readonly MassTransitIdentityToken _token;
        private readonly ILogger<AnotherTestIdentityConsumer> _logger;

        public AnotherTestIdentityConsumer(MassTransitIdentityToken token, 
            ILogger<AnotherTestIdentityConsumer> logger)
        {
            _token = token;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<AnotherIdentityExampleRequest> context)
        {
            await context.RespondAsync<AnotherIdentityExampleResponse>(new
            {
                Token = _token.Value
            });

            _logger.LogInformation($"Consumed operation with token {_token.Value}");
        }
    }
}
