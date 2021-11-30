using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReversePhoneLookup.Abstract.Services;
using ReversePhoneLookup.Models.Requests;

namespace ReversePhoneLookup.Web.Controllers
{
    [ApiController]
    public class PhoneController : ControllerBase
    {
        private readonly ILookupService lookupService;

        public PhoneController(ILookupService lookupService)
        {
            this.lookupService = lookupService;
        }

        [HttpGet("/lookup")]
        public async Task<IActionResult> Lookup([FromQuery] LookupRequest request, CancellationToken cancellationToken)
        {
            var result = await lookupService.LookupAsync(request, cancellationToken);
            return Ok(result);
        }

        [HttpPost("/phone")]
        public async Task<IActionResult> AddPhone([FromBody] AddPhoneRequest request, CancellationToken cancellationToken)
        {
            var result = await lookupService.AddContactWithPhoneAsync(request, cancellationToken); 
            return Ok(result);
        }

    }
}
