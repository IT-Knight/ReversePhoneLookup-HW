using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReversePhoneLookup.Abstract.Repositories;
using ReversePhoneLookup.Api.Models.Entities;

namespace ReversePhoneLookup.Api.Repositories
{
    public class PhoneRepository : IPhoneRepository
    {
        private readonly PhoneLookupContext context;

        public PhoneRepository(PhoneLookupContext context)
        {
            this.context = context;
        }

        public async Task<Phone> GetPhoneDataAsync(string phone, CancellationToken cancellationToken)
        {
            return await context.Phones
                .Where(x => x.Value == phone)
                .Include(x => x.Operator)
                .Include(x => x.Contacts)
                .FirstOrDefaultAsync(cancellationToken);
        }


        public async Task<Phone> AddPhoneAsync(Phone phone)
        {
            await context.Phones.AddAsync(phone);
            await context.SaveChangesAsync();
            return phone;
        }

        public async Task<bool> IsPhoneExists(string phone, CancellationToken cancellationToken)
        {
            var phoneObject = await context.Phones.FirstOrDefaultAsync(x => x.Value == phone, cancellationToken);
            return phoneObject != null;
        }
    }
}
