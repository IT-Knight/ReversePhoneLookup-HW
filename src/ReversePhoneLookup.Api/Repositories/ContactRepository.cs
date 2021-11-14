using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReversePhoneLookup.Abstract.Repositories;
using ReversePhoneLookup.Api.Models.Entities;

namespace ReversePhoneLookup.Api.Repositories
{
    public class ContactRepository : IContactRepository
    {
        private readonly PhoneLookupContext context;

        public ContactRepository(PhoneLookupContext context)
        {
            this.context = context;
        }
        
        public async Task<Contact> GetContactAsync(string name, CancellationToken cancellationToken)
        {
            return await context.Contacts.FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
        }

        public async Task<Contact> AddContactAsync(Contact newContact)
        {
            await context.Contacts.AddAsync(newContact);
            await context.SaveChangesAsync();
            return newContact;
        }

        public async Task<bool> IsContactExists(string name)
        {
            var contact = await context.Contacts.FirstOrDefaultAsync(x => x.Name == name);
            return contact != null;
        }
        
    }
}
