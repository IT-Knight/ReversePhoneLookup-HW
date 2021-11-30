using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReversePhoneLookup.Api.Models.Entities;
using ReversePhoneLookup.IntegrationTests.Common;
using ReversePhoneLookup.Models.Requests;
using ReversePhoneLookup.Models.Responses;
using Xunit;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ReversePhoneLookup.IntegrationTests.Controllers
{
    public class PhoneControllerTests : IClassFixture<CustomWebApplicationFactory<StartupSUT>>
    {
        private readonly HttpClient client;

        public PhoneControllerTests(CustomWebApplicationFactory<StartupSUT> factory)
        {
            client = factory.CreateClient();
        }

        [Fact]
        public async Task Lookup_ValidData_ShouldReturn200()
        {
            using (var fixture = new DbFixture())
            {
                fixture.DbContext.Phones.Add(new Phone()
                {
                    Value = "+37367123456",
                    Operator = new Operator()
                    {
                        Mcc = "123",
                        Mnc = "99",
                        Name = "Test"
                    },
                    Contacts = new List<Contact>()
                    {
                        new Contact() { Name = "User" }
                    }
                });
                await fixture.DbContext.SaveChangesAsync();

                string url = "/lookup?phone=67123456";

                var response = await client.GetAsync(url);

                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                JToken json = JToken.Parse(jsonResponse);

                Assert.Equal("+37367123456", json["phone"]);
            }
        }

        [Fact]
        public async Task Phone_ValidData_ShouldReturnStatusOkWithLookupResponseObject() 
        {
            // Arrange
            using var fixture = new DbFixture();  
            string url = "/phone";
            
            var request = new AddPhoneRequest { Phone = "+37360658132", Names = new List<string> { "Malder", "Scaly", "Jhon" } };
            HttpContent content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            
            // Act
            var response = await client.PostAsync(url, content);
            LookupResponse objResult = JsonConvert.DeserializeObject<LookupResponse>(await response.Content.ReadAsStringAsync());
            // Built-in System.Text.Json cannot deserialize this! Why?
            
            // надо + и базу данных проверять, т.к. интеграционные.
            
            // TODO: проверить что попало в базу данных.
            
            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(objResult.Names, request.Names);
            Assert.Equal(objResult.Phone, request.Phone);
            Assert.Null(objResult.Operator);
        }
    }
}
