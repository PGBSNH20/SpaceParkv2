using System.Linq;
using SpacePark_API.DataAccess;
using SpacePark_API.Models;
using System.Threading.Tasks;
using Xunit;
using SpacePark_API.Controllers;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;
using SpacePark_API;
using SpacePark_API.Networking;
using System.Text;
using SpacePark_API.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Net.Http.Headers;

namespace SpaceParkTests
{
    public class WebRequestTests : IClassFixture<FakeWebHost<Startup>>
    {
        private AccountController _controller;
        private IStarwarsRepository _repository;
        private FakeWebHost<Startup> _fakeHost;
        public HttpClient Client { get; }
        public WebRequestTests(FakeWebHost<Startup> fakeHost)
        {
            _fakeHost = fakeHost;
            Client = fakeHost.CreateClient();
            _repository = new DbRepository(fakeHost.InMemoryContext);
            _controller = new AccountController(_repository, fakeHost.Configuration);
        }
        [Fact]
        public void GetPersonFromDb_expectLuke() //from database
        {
            var xWing = new SpaceShip { Model = "X-Wing" };
            var luke = new Person() { Name = "Luke Skywalker" };
            var LukeAccount = new Account() { SpaceShip = xWing, Person = luke, AccountName = "LuckyLuke" };

            _fakeHost.InMemoryContext.Add(LukeAccount);
            _fakeHost.InMemoryContext.SaveChanges();

            var model = _repository.People.FirstOrDefault(p => p.Name == "Luke Skywalker");
            Assert.Equal("Luke Skywalker", model.Name);
        }
        [Fact]
        public void GetAccountFromDb_expectLuke()
        {
            var xWing = new SpaceShip { Model = "X-Wing" };
            var luke = new Person() { Name = "Luke Skywalker" };
            var LukeAccount = new Account() { SpaceShip = xWing, Person = luke, AccountName = "LuckyLuke" };

            _fakeHost.InMemoryContext.Add(LukeAccount);
            _fakeHost.InMemoryContext.SaveChanges();

            var model = _repository.Accounts.FirstOrDefault(p => p.AccountName == "LuckyLuke");
            Assert.Equal("LuckyLuke", model.AccountName);
        }
        [Fact]
        public void GetLukeSkywalkerFromSwapi_ExpectPerson()
        {
            var luke = APICollector.ParseUser("LukeSkywalker");
            Assert.True(luke is not null);
            Assert.IsType<Person>(luke);
            Assert.Equal("Luke Skywalker", luke.Name);
        }
        [Fact]
        public async Task GetSpaceShipsFromSwapi_ExpectList()
        {
            var spaceShips = APICollector.ReturnShips();
            Assert.True(spaceShips.Count() != 0);
            Assert.IsType<SpaceShip>(spaceShips[0]);
        }
        [Fact]
        public async Task RegisterC3POWithClient_ExpectSucess()
        {
            var darthVader = new
            {
                name = "c-3po",
                spaceShipModel = "BTL Y-wing",
                accountName = "RoughMetal33",
                password = "IHATERUST_DEATHTORUST@123"
                
            };
            var json = JsonConvert.SerializeObject(darthVader);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await Client.PostAsync("/api/Account/Register", data);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task LoginDarthVaderWithClient_ExpectSucess()
        {
            var darthVader = new Account()
            {
                AccountName = "BigBlack",
                Password = "Darth@123",
                Role = Role.User
            };
            _repository.Add(darthVader);
            _repository.SaveChanges();
            var json = JsonConvert.SerializeObject(new
            {
                username = "BigBlack",
                password = "Darth@123"
            });
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await Client.PostAsync("/api/Account/Login", data).ConfigureAwait(continueOnCapturedContext: false);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task RegisterDarthMaulWithClient_ExpectSucess()
        {
            var darthVader = new
            {
                name = "darth maul",
                spaceShipModel = "CR90 Corvette",
                accountName = "RedBlack",
                password = "Maul@123"
            };
            var json = JsonConvert.SerializeObject(darthVader);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await Client.PostAsync("/api/Account/Register", data);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task RegisterDarthVaderInApp_ExpectSucess()
        {
            var darthVader = new RegisterModel
            {
                Name = "darth vader",
                SpaceShipModel = "BTL Y-wing",
                AccountName = "BigBlack",
                Password = "Darth@123"
            };

            if (_controller.Register(darthVader) is ObjectResult result) 
                Assert.Equal(200, result.StatusCode);
        }
        [Fact]
        public async Task LoginDarthVaderWithClient_ExpectToken()
        {
            var darthVader = new Account()
            {
                AccountName = "BigBlack",
                Password = "Darth@123",
                Role = Role.User
            };
            _repository.Add(darthVader);
            _repository.SaveChanges();
            var json = JsonConvert.SerializeObject(new
            {
                username = "BigBlack",
                password = "Darth@123"
            });
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await Client.PostAsync("/api/Account/Login", data).ConfigureAwait(continueOnCapturedContext: false);
            var responseText = string.Empty;
            if (response.IsSuccessStatusCode)
            {
                responseText = await response.Content.ReadAsStringAsync();
            }
            var responseValuePairs = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseText);

            // Did the response come with an access token?
            Assert.True(responseValuePairs != null && responseValuePairs.ContainsKey("token"));

        }
        [Fact]
        public async Task ParkWithDarthVader_ExpectCorrectPrice()
        {
            var spaceShip = new SpaceShip() { ShipLength = "5" };
            var darthVader = new Account()
            {
                AccountName = "BigBlack",
                Password = "Darth@123",
                SpaceShip = spaceShip,
                Role = Role.User,
                Person = new Person() { Name = "Darth Vader"}
            };
            var spacePort = new SpacePort("PortRoyal", 3, 10.0);

            _repository.Add(darthVader);
            _repository.Add(spacePort);
            _repository.SaveChanges();

            var loginJson = JsonConvert.SerializeObject(new
            {
                username = "BigBlack",
                password = "Darth@123"
            });
            var loginBodyData = new StringContent(loginJson, Encoding.UTF8, "application/json");

            var loginResponse = await Client.PostAsync("/api/Account/Login", loginBodyData).ConfigureAwait(continueOnCapturedContext: false);
            if (!loginResponse.IsSuccessStatusCode) throw new Exception("Should get 200");

            var loginResponseText = await loginResponse.Content.ReadAsStringAsync();
            var loginResponseValuePairs = JsonConvert.DeserializeObject<Dictionary<string, string>>(loginResponseText);

            var token = loginResponseValuePairs.GetValueOrDefault("token");
            if (token == null) throw new Exception("Should get token");

            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var parkJson = JsonConvert.SerializeObject(new
            {
                minutes = 10,
                spacePortName = "PortRoyal"
            });
            var parkBodyData = new StringContent(parkJson, Encoding.UTF8, "application/json");

            var parkResponse = await Client.PostAsync("/api/Parking/Park", parkBodyData).ConfigureAwait(continueOnCapturedContext: false);

            if (!loginResponse.IsSuccessStatusCode) throw new Exception("Should get 200");

            var parkResponseText = await parkResponse.Content.ReadAsStringAsync();
            var parkResponseProps = JsonConvert.DeserializeObject<Receipt>(parkResponseText);

            var expected = spacePort.CalculatePrice(spaceShip, 10);
            Assert.Equal(expected, parkResponseProps.Price);

        }

        [Fact]
        public async Task AttemptToAddSpacePortWithoutAdminRights_ExpectFail()
        {
            var darthVader = new Account()
            {
                AccountName = "BigBlack",
                Password = "Darth@123",
                Role = Role.User,
                Person = new Person() { Name = "Darth Vader"}
            };
            _repository.Add(darthVader);
            _repository.SaveChanges();
            var loginJson = JsonConvert.SerializeObject(new
            {
                username = "BigBlack",
                password = "Darth@123"
            });
            var loginBodyData = new StringContent(loginJson, Encoding.UTF8, "application/json");

            var loginResponse = await Client.PostAsync("/api/Account/Login", loginBodyData).ConfigureAwait(continueOnCapturedContext: false);
            if (!loginResponse.IsSuccessStatusCode) throw new Exception("Should get 200");

            var loginResponseText = await loginResponse.Content.ReadAsStringAsync();
            var loginResponseValuePairs = JsonConvert.DeserializeObject<Dictionary<string, string>>(loginResponseText);

            var token = loginResponseValuePairs.GetValueOrDefault("token");
            if (token == null) throw new Exception("Should get token");
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            var newParkJson = JsonConvert.SerializeObject(new
            {
                name = "EmoParking",
                parkingSpots = 1337,
                priceMultiplier = 69
            });
            var parkBodyData = new StringContent(newParkJson, Encoding.UTF8, "application/json");

            var parkResponse = await Client.PostAsync("/api/Admin/AddSpacePort", parkBodyData).ConfigureAwait(continueOnCapturedContext: false);
            Assert.Equal(HttpStatusCode.Forbidden, parkResponse.StatusCode);
        }
        [Fact]
        public async Task AttemptToAddSpacePortWithAdminRights_ExpectFail()
        {
            var c3po = new Account()
            {
                AccountName = "rustSlayer33",
                Password = "NoRust!!@123",
                Role = Role.Administrator,
                Person = new Person() { Name = "C-3PO"}
            };
            _repository.Add(c3po);
            _repository.SaveChanges();
            var loginJson = JsonConvert.SerializeObject(new
            {
                username = "rustSlayer33",
                password = "NoRust!!@123"
            });
            var loginBodyData = new StringContent(loginJson, Encoding.UTF8, "application/json");

            var loginResponse = await Client.PostAsync("/api/Account/Login", loginBodyData).ConfigureAwait(continueOnCapturedContext: false);
            if (!loginResponse.IsSuccessStatusCode) throw new Exception("Should get 200");

            var loginResponseText = await loginResponse.Content.ReadAsStringAsync();
            var loginResponseValuePairs = JsonConvert.DeserializeObject<Dictionary<string, string>>(loginResponseText);

            var token = loginResponseValuePairs.GetValueOrDefault("token");
            if (token == null) throw new Exception("Should get token");
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            var newParkJson = JsonConvert.SerializeObject(new
            {
                name = "Rust-free parking",
                parkingSpots = 1337,
                priceMultiplier = 69
            });
            var parkBodyData = new StringContent(newParkJson, Encoding.UTF8, "application/json");

            var parkResponse = await Client.PostAsync("/api/Admin/AddSpacePort", parkBodyData).ConfigureAwait(continueOnCapturedContext: false);
            Assert.Equal(HttpStatusCode.OK, parkResponse.StatusCode);
        }
    }
}
