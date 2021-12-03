using System;
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
    public class PhoneRepositoryTests : IClassFixture<CustomWebApplicationFactory<StartupSUT>>, IDisposable
    {
        private readonly DbFixture fixture;
        private readonly PhoneLookupContext context;
        private readonly PhoneRepository sut;
        private readonly Phone phone;

        public PhoneRepositoryTests()
        {
            fixture = new DbFixture();
            context = fixture.DbContext;
            sut = new PhoneRepository(context);
            phone = new Phone { Value = "060658132" };
        }
        
        public void Dispose()
        {
            fixture.Dispose();
            GC.SuppressFinalize(this);
        }

        private async Task InitialAddPhone()
        {
            await context.Phones.AddAsync(phone);
            await context.SaveChangesAsync();
        }

        private async Task DeletePhoneCleanup()
        {
            context.Phones.Remove(phone);
            await context.SaveChangesAsync();
        }
        
        [Fact, TestPriority(1)]
        public async Task AddPhoneAsync_ShouldReturnSinglePhoneWithNotZeroIdAndNumberEqualsInput()
        {
            // Arrange

            // Act
            await sut.AddPhoneAsync(phone);
            var phoneEntities = await context.Phones.Where(x => x.Value == phone.Value).ToListAsync();

            // Assert
            Assert.Single(phoneEntities);
            Assert.NotEqual(0, phoneEntities[0].Id);
            Assert.Equal(phone.Value, phoneEntities[0].Value);

            await DeletePhoneCleanup();
        }

        [Fact, TestPriority(2)]
        public async Task GetPhoneDataAsync_PhoneExist_ShouldReturnPhoneObject()
        {
            // Arrange
            await InitialAddPhone();
            
            // Act
            var result = await sut.GetPhoneDataAsync(phone.Value, CancellationToken.None);
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(phone.Value, result.Value);

            await DeletePhoneCleanup();
        }
        
        [Fact, TestPriority(3)]
        public async Task GetPhoneDataAsync_PhoneNotExist_ShouldReturnNull()
        {
            var result = await sut.GetPhoneDataAsync(phone.Value, CancellationToken.None);
            Assert.Null(result);
        }

        [Fact, TestPriority(4)]
        public async Task IsPhoneExists_PhoneExists_ReturnsTrue()
        {
            // Arrange
            await InitialAddPhone();

            // Act
            var result = await sut.IsPhoneExists(phone.Value, CancellationToken.None);
            
            // Assert
            Assert.True(result);
            
            await DeletePhoneCleanup();
        }
        
        [Fact, TestPriority(5)]
        public async Task IsPhoneExists_PhoneExists_ReturnsFalse()
        {
            var result = await sut.IsPhoneExists(phone.Value, CancellationToken.None);
            Assert.False(result);
        }
    }
}