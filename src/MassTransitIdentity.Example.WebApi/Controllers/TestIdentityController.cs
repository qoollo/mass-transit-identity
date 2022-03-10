using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransitIdentity.Core;
using MassTransitIdentity.Example.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MassTransitIdentity.Example.Controllers
{
    [ApiController]
    [Route("api/identity")]
    public class TestIdentityController : ControllerBase
    {
        private readonly ILogger<TestIdentityController> _logger;
        private MassTransitIdentityToken _token;
        private readonly IRequestClient<IdentityExampleRequest> _requestClient;

        public TestIdentityController(ILogger<TestIdentityController> logger,
            IRequestClient<IdentityExampleRequest> requestClient,
            MassTransitIdentityToken token)
        {
            _requestClient = requestClient;
            _logger = logger;
            _token = token;
        }

        [HttpPost]
        [Route("/act")]
        public async Task<IActionResult> Act([FromQuery] Guid? id)
        {
            try
            {
                Guid? clientId = id;
                _token.Value = clientId;

                var response = await _requestClient.GetResponse<IdentityExampleResponse>(new {});

                return Ok(new {response.Message.Token});
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return StatusCode(500, e);
            }
        }
    }
}
