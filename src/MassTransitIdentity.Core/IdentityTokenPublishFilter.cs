using MassTransit;
using Microsoft.Extensions.Logging;

namespace MassTransitIdentity.Core
{
    public class IdentityTokenPublishFilter<T> : IFilter<PublishContext<T>> where T : class
    {
        private MassTransitIdentityToken _token;
        private readonly ILogger<IdentityTokenPublishFilter<T>> _logger;

        public IdentityTokenPublishFilter(MassTransitIdentityToken token, ILogger<IdentityTokenPublishFilter<T>> logger)
        {
            _token = token;
            _logger = logger;
        }

        public void Probe(ProbeContext context)
        {

        }

        public Task Send(PublishContext<T> context, IPipe<PublishContext<T>> next)
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
