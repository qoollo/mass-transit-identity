using MassTransit;
using MassTransitIdentity.Example.Contracts;
using System.Threading.Tasks;

namespace MassTransitIdentity.Example.ConsumerService
{
    public class TestIdentityConsumer : IConsumer<IdentityExampleRequest>
    { 
        private readonly IRequestClient<AnotherIdentityExampleRequest> _requestClient;

        public TestIdentityConsumer(IRequestClient<AnotherIdentityExampleRequest> requestClient)
        {
            _requestClient = requestClient;
        }

        public async Task Consume(ConsumeContext<IdentityExampleRequest> context)
        {
            var response = await _requestClient.GetResponse<AnotherIdentityExampleResponse>(new { });

            await context.RespondAsync<IdentityExampleResponse>(new
            {
                Token = response.Message.Token
            });
        }
    }
}
