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

namespace ReversePhoneLookup.IntegrationTests
{
    public class PhoneRepositoryTests : IClassFixture<CustomWebApplicationFactory<StartupSUT>>, IDisposable
    {
        private readonly DbFixture fixture;
        private readonly PhoneLookupContext _context;
        private readonly PhoneRepository sut;
        private readonly Phone phone;

        public PhoneRepositoryTests()
        {
            fixture = new DbFixture();
            _context = fixture.DbContext;
            sut = new PhoneRepository(_context);
            phone = new Phone { Value = "060658132" };
        }
        
        public void Dispose()
        {
            fixture.Dispose();
            GC.SuppressFinalize(this);
        }

        [Fact]
        public async Task AddPhoneAsync_ShouldReturnSinglePhoneWithNotZeroIdAndNumberEqualsInput()
        {
            // Arrange

            // Act
            await sut.AddPhoneAsync(phone);
            var phoneEntities = await _context.Phones.Where(x => x.Value == phone.Value).ToListAsync();

            // Assert
            Assert.Single(phoneEntities);
            Assert.NotEqual(0, phoneEntities[0].Id);
            Assert.Equal(phone.Value, phoneEntities[0].Value);

            await DeletePhoneCleanup();  // Does all tests run async? can be conflicts?
        }

        [Fact]
        public async Task GetPhoneDataAsync_PhoneExist_ShouldReturnPhoneObject()
        {
            // Arrange
            await _context.Phones.AddAsync(phone);
            await _context.SaveChangesAsync();
            
            // Act
            var result = await sut.GetPhoneDataAsync(phone.Value, CancellationToken.None);
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(phone.Value, result.Value);

            await DeletePhoneCleanup();
        }
        
        [Fact]
        public async Task GetPhoneDataAsync_PhoneNotExist_ShouldReturnNull()
        {
            var result = await sut.GetPhoneDataAsync(phone.Value, CancellationToken.None);
            Assert.Null(result);
        }

        [Fact]
        public async Task IsPhoneExists_PhoneExists_ReturnsTrue()
        {
            // Arrange
            await _context.Phones.AddAsync(phone);
            await _context.SaveChangesAsync();

            // Act
            var result = await sut.IsPhoneExists(phone.Value, CancellationToken.None);
            
            // Assert
            Assert.True(result);
            
            await DeletePhoneCleanup();
        }
        [Fact]
        public async Task IsPhoneExists_PhoneExists_ReturnsFalse()
        {
            var result = await sut.IsPhoneExists(phone.Value, CancellationToken.None);
            Assert.False(result);
        }
        
        
        private async Task DeletePhoneCleanup()
        {
            _context.Phones.Remove(phone);
            await _context.SaveChangesAsync();
        }
    }
}