using MassTransit;
using Microsoft.Extensions.Logging;

namespace MassTransitIdentity.Core
{
    public class IdentityTokenConsumeFilter<T> : IFilter<ConsumeContext<T>> where T : class
    {
        private MassTransitIdentityToken _token;
        private readonly ILogger<IdentityTokenConsumeFilter<T>> _logger;

        public IdentityTokenConsumeFilter(MassTransitIdentityToken token,
            ILogger<IdentityTokenConsumeFilter<T>> logger)
        {
            _token = token;
            _logger = logger;
        }

        public void Probe(ProbeContext context)
        {

        }

        public Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            if (Guid.TryParse(context.Headers.Get<string>(nameof(MassTransitIdentityToken)), out Guid clientId))
            {
                _token.Value = clientId;
                _logger.LogDebug("Parsed client: {TokenValue}.", _token.Value);
            }

            return next.Send(context);
        }
    }
}
