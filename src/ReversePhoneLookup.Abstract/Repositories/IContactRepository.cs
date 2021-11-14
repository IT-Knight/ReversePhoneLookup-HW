using System.Threading;
using System.Threading.Tasks;
using ReversePhoneLookup.Api.Models.Entities;

namespace ReversePhoneLookup.Abstract.Repositories
{
    public interface IContactRepository
    {
        Task<Contact> GetContactAsync(string name, CancellationToken cancellationToken);

        Task<Contact> AddContactAsync(Contact contact);

        Task<bool> IsContactExists(string name);
    }
}
