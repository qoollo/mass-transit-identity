using MassTransit;
using Microsoft.Extensions.Logging;

namespace MassTransitIdentity.Core
{
    public class IdentityTokenSendFilter<T> : IFilter<SendContext<T>> where T : class
    {
        private MassTransitIdentityToken _token;
        private readonly ILogger<IdentityTokenSendFilter<T>> _logger;

        public IdentityTokenSendFilter(MassTransitIdentityToken token, ILogger<IdentityTokenSendFilter<T>> logger)
        {
            _token = token;
            _logger = logger;
        }

        public void Probe(ProbeContext context)
        {

        }

        public Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
        {
            if (_token.Value != null && _token.Value != Guid.Empty)
            {
                context.Headers.Set(nameof(MassTransitIdentityToken), _token.Value.Value.ToString());
                _logger.LogDebug($"Added {nameof(MassTransitIdentityToken)} with value: {_token.Value}.");
            }

            return next.Send(context);
        }
    }
}
