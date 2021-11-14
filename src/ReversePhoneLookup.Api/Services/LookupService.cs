using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using ReversePhoneLookup.Abstract.Repositories;
using ReversePhoneLookup.Abstract.Services;
using ReversePhoneLookup.Api.Models.Entities;
using ReversePhoneLookup.Models;
using ReversePhoneLookup.Models.Exceptions;
using ReversePhoneLookup.Models.Requests;
using ReversePhoneLookup.Models.Responses;

namespace ReversePhoneLookup.Api.Services
{
    public class LookupService : ILookupService
    {
        private readonly IPhoneService phoneService;
        private readonly IPhoneRepository phoneRepository;
        private readonly IContactRepository contactRepository;

        public LookupService(IPhoneService phoneService, IPhoneRepository phoneRepository, IContactRepository contactRepository)
        {
            this.phoneService = phoneService;
            this.phoneRepository = phoneRepository;
            this.contactRepository = contactRepository;
        }

        public async Task<LookupResponse> LookupAsync(LookupRequest request, CancellationToken cancellationToken)
        {
            string phone = phoneService.TryFormatPhoneNumber(request.Phone);
            if (!phoneService.IsPhoneNumber(phone))
                throw new ApiException(StatusCode.InvalidPhoneNumber);

            var data = await phoneRepository.GetPhoneDataAsync(phone, cancellationToken);
            if (data == null)
                throw new ApiException(StatusCode.NoDataFound);

            return new LookupResponse()
            {
                Phone = phone,
                Operator = data.Operator == null ? null : new OperatorResponse()
                {
                    Name = data.Operator.Name,
                    Code = data.Operator.Mcc + data.Operator.Mnc
                },
                Names = data.Contacts?.Select(x => x.Name).ToList()
            };
        }

        public async Task<LookupResponse> AddContactWithPhoneAsync(AddPhoneRequest request,
            CancellationToken cancellationToken)
        {
            string phone = phoneService.TryFormatPhoneNumber(request.Phone);
            List<string> names = request.Names;

            if (string.IsNullOrEmpty(phone) || !phoneService.IsPhoneNumber(phone))
                throw new ApiException(StatusCode.InvalidPhoneNumber);

            if (names.Any(string.IsNullOrEmpty))
                throw new ApiException(StatusCode.InvalidPersonName);


            var phoneCurrentlyExists = await phoneRepository.IsPhoneExists(phone, cancellationToken);

            await ProcessContactsAsync(names, phoneCurrentlyExists, phone, cancellationToken);

            return new LookupResponse {Phone = phone, Names = names, Operator = null};
        }


        private async Task ProcessContactsAsync(List<string> names, bool phoneExists, string phoneString, CancellationToken cancellationToken)
        {
            Phone phone = phoneExists switch
            {
                true => await phoneRepository.GetPhoneDataAsync(phoneString, cancellationToken),
                false => await phoneRepository.AddPhoneAsync(new Phone {Value = phoneString})
            };

            foreach (var name in names)
            {
                var contactExists = await contactRepository.IsContactExists(name);

                if (!contactExists)
                {
                    await contactRepository.AddContactAsync(new Contact
                    {
                        Name = name, 
                        PhoneId = phone.Id
                    });
                }

            }
        }
    }
}
