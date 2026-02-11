using Microsoft.AspNetCore.Mvc;
using Shared.Contracts.Duplicata;

namespace Duplicata.ApiGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DuplicatasController : ControllerBase
    {
        private readonly DuplicataService.DuplicataServiceClient _grpc;

        public DuplicatasController(DuplicataService.DuplicataServiceClient duplicataServiceClient)
        {
            _grpc = duplicataServiceClient;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateDuplicataRequest request)
        {
            var response = await _grpc.CreateDuplicataAsync(request);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _grpc.GetDuplicatasAsync(new Empty());
            return Ok(response);
        }

        [HttpGet("by-number")]
        public async Task<IActionResult> GetByNumber([FromQuery] string numero)
        {
            var response = await _grpc.GetDuplicataByNumberAsync(new GetDuplicataByNumberRequest { Numero = numero });
            return Ok(response);
        }
    }
}
