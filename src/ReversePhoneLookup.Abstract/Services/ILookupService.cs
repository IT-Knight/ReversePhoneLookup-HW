using System.Threading;
using System.Threading.Tasks;
using ReversePhoneLookup.Models.Requests;
using ReversePhoneLookup.Models.Responses;

namespace ReversePhoneLookup.Abstract.Services
{
    public interface ILookupService
    {
        Task<LookupResponse> LookupAsync(LookupRequest request, CancellationToken cancellationToken);

        Task<LookupResponse> AddContactWithPhoneAsync(AddPhoneRequest request, CancellationToken cancellationToken);
    }
}
