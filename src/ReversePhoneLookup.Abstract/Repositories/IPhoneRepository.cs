using System.Threading;
using System.Threading.Tasks;
using ReversePhoneLookup.Api.Models.Entities;

namespace ReversePhoneLookup.Abstract.Repositories
{
    public interface IPhoneRepository
    {
        Task<Phone> GetPhoneDataAsync(string phone, CancellationToken cancellationToken);

        Task<Phone> AddPhoneAsync(Phone phone);

        Task<bool> IsPhoneExists(string phone, CancellationToken cancellationToken);
    }
}
