﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReversePhoneLookup.Api;
using ReversePhoneLookup.Api.Models.Entities;
using ReversePhoneLookup.Api.Repositories;
using ReversePhoneLookup.IntegrationTests.Common;
using Xunit;

namespace ReversePhoneLookup.IntegrationTests.Repositories
{
    [TestCaseOrderer("ReversePhoneLookup.IntegrationTests.Common.PriorityOrderer", "ReversePhoneLookup")]
    public class ContactRepositoryTests : IClassFixture<CustomWebApplicationFactory<StartupSUT>>, IDisposable
    {
        private readonly DbFixture fixture;
        private readonly PhoneLookupContext context;
        private readonly ContactRepository sut;
        private readonly Contact contact;

        public ContactRepositoryTests()
        {
            fixture = new DbFixture();
            context = fixture.DbContext;
            sut = new ContactRepository(context);
            var phoneId = InitializeBasePhoneValue();
            contact = new Contact {Name = "Agent Malder", PhoneId = phoneId};
        }

        private int InitializeBasePhoneValue()
        {
            var phone = new Phone {Value = "060658132"};
            context.Phones.Add(phone);
            context.SaveChanges();
            return phone.Id;
        }

        private async Task InitialAddContact()
        {
            await context.AddAsync(contact);
            await context.SaveChangesAsync();
        }

        private async Task DeleteContactCleanup()
        {
            context.Contacts.Remove(contact);
            await context.SaveChangesAsync();
        }

        public void Dispose()
        {
            fixture.Dispose();
        }

        [Fact, TestPriority(1)]
        public async Task AddContactAsync_ShouldReturnSingleContactWithNotZeroIdAndNameEqualsInput()
        {
            // Arrange

            // Act
            await sut.AddContactAsync(contact);
            
            var contactEntities = await context.Contacts.Where(x => x.Name == contact.Name).ToListAsync();

            // Assert
            Assert.Single(contactEntities);
            Assert.NotEqual(0, contactEntities[0].Id);
            Assert.Equal(contact.Name, contactEntities[0].Name);
            await DeleteContactCleanup();
        }

        [Fact, TestPriority(2)]
        public async Task GetContactAsync_ContactExist_ReturnsContactEntity()
        {
            // Arrange
            await InitialAddContact();

            // Act
            var result = await sut.GetContactAsync(contact.Name, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(contact.Name, result.Name);
            await DeleteContactCleanup();
        }

        [Fact, TestPriority(3)]
        public async Task GetContactAsync_ContactNotExist_ReturnsNull()
        {
            var result = await sut.GetContactAsync(contact.Name, CancellationToken.None);
            Assert.Null(result);
        }

        [Fact, TestPriority(4)]
        public async Task IsContactExists_ContactExists_ReturnsTrue()
        {
            // Arrange
            await InitialAddContact();

            // Act
            var result = await sut.IsContactExists(contact.Name);
            
            // Arrange
            Assert.True(result);
            await DeleteContactCleanup();
        }

        [Fact, TestPriority(5)]
        public async Task IsContactExists_ContactNotExists_ReturnsFalse()
        {
            Assert.False(await sut.IsContactExists(contact.Name));
        }
    }
}