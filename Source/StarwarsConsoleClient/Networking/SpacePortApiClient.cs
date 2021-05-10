using Newtonsoft.Json;
using StarwarsConsoleClient.Main;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StarwarsConsoleClient.Networking
{
    public partial class SpacePortApiClient
    {
        private HttpClient _client;
        public SpacePortApiClient(string baseUrl)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
            _client = client;
        }

        private static class EndPoints
        {
            public static class Account
            {
                public static readonly string register =            @"/api/Account/Register";
                public static readonly string login =               @"/api/Account/Login";
                public static readonly string changeSpaceShip =     @"/api/Account/ChangeSpaceShip";
                public static readonly string getHomeworld =        @"​/api​/Account​/GetHomeworld";
                public static readonly string Ships =               @"​/api/Account/Ships?maxLength=150";
                public static readonly string MyShip =              @"/api/Account/MySpaceShip";
                public static readonly string MyData =              @"/api/Account/MyData";
            }
            public static class Parking
            {
                public static readonly string park =                @"​/api​/Parking/Park";
                public static readonly string getSpacePorts =       @"​/api​/Parking​/GetSpacePorts";
                public static readonly string parkingPrice =        @"​/api​/Parking​/Price";

            }
            public static class Admin
            {
                public static readonly string Accounts =/*        */@"​/api​/Admin/Accounts";
                public static readonly string getAccount =/*      */@"​/api​/Admin/Accounts";
                public static readonly string getAccounts =         @"​/api​/Admin/GetAccount";
                public static readonly string addSpacePort =        @"​/api​/Admin/AddSpacePort";
                public static readonly string SpacePortEnabled =    @"​/api​/Admin/SpacePortEnabled";
                public static readonly string DeleteSpacePort =     @"​/api​/Admin/DeleteSpacePort";
                public static readonly string UpdateSpacePortPrice =@"​/api​/Admin/UpdateSpacePortPrice";
                public static readonly string UpdateSpacePortName = @"​/api​/Admin/UpdateSpacePortName";
                public static readonly string PromoteAdmin =        @"​/api​/Admin/PromoteAdmin";
                public static readonly string DemoteAdmin =         @"​/api​/Admin/DemoteAdmin";
            }
        }

        private async Task<HttpResponseMessage> JsonPostRequestAsync(string endpoint, object obj)
        {
            string json = "";
            if(obj != null) json = JsonConvert.SerializeObject(obj);

            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(endpoint, data).ConfigureAwait(continueOnCapturedContext: false);
            return response;
        }
        private async Task<HttpResponseMessage> GetRequestAsync(string endpoint, string queryString = "")
        {
            var response = await _client.GetAsync(endpoint + queryString);
            return response;
        }
        private string QueryStringParse(object queryObj)
        {
            NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
            PropertyInfo[] infos = queryObj.GetType().GetProperties();
            foreach (var info in infos)
            {
                queryString.Add(info.Name, info.GetValue(queryObj, null).ToString());
            }
            return '?' + queryString.ToString();
        }

        #region Account Requests
        public async Task<bool> RegisterAsync(string spaceShipModel, string name, string accountName, string password)
        {
            var response = await JsonPostRequestAsync(EndPoints.Account.register, new
            {
                name,
                accountName,
                password,
                spaceShipModel,
            });

            return response.IsSuccessStatusCode;
        }
        public async Task<bool> LoginAsync(string accountName, string password)
        {
            var response = await JsonPostRequestAsync(EndPoints.Account.login, new
            {
                username = accountName,
                password = password
            });
            var responseContentString = await response.Content.ReadAsStringAsync();
            var token = GetResponseValueWithKey("token", responseContentString);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return response.IsSuccessStatusCode;
        }
        private string GetResponseValueWithKey(string key, string responseContentString)
        {
            var loginResponseValuePairs = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseContentString);
            var val = loginResponseValuePairs.GetValueOrDefault(key);
            return val;
        }
        public async Task<bool> ChangeSpaceShipAsync(string uri, string spaceshipModel)
        {
            var response = await JsonPostRequestAsync(uri + spaceshipModel, null);
            return response.IsSuccessStatusCode;
        }
        public async Task<Homeworld> GetHomeworldAsync(string uri)
        {
            var response = await JsonPostRequestAsync(uri, new
            {
            });
            var homeplanet = JsonConvert.DeserializeObject<Homeworld>(await response.Content.ReadAsStringAsync());
            return homeplanet;
        }
        public async Task<SpaceShip> MySpaceShipAsync()
        {
            var response = await JsonPostRequestAsync(EndPoints.Account.MyShip, null);
            var responseContentString = await response.Content.ReadAsStringAsync();
            var spaceShip = JsonConvert.DeserializeObject<SpaceShip>(responseContentString);
            return spaceShip;
        }
        public async Task<Person> MyDataAsync()
        {
            var response = await JsonPostRequestAsync(EndPoints.Account.MyData, null);
            var responseContentString = await response.Content.ReadAsStringAsync();
            var person = JsonConvert.DeserializeObject<Person>(responseContentString);
            return person;
        }
        public async Task<IEnumerable<SpaceShip>> GetShipsAsync(string uri)
        {
            var response = await GetRequestAsync(uri);
            var responseContentString = await response.Content.ReadAsStringAsync();
            var ships = JsonConvert.DeserializeObject<IEnumerable<SpaceShip>>(responseContentString);
            return ships;
        }
        public async Task<double> GetPriceAsync(string uri)
        {

            var response = await GetRequestAsync(uri);
            var responseContentString = await response.Content.ReadAsStringAsync();
            var price = JsonConvert.DeserializeObject<double>(responseContentString);
            return price;
        }
        #endregion
        #region Parking Requests
        public async Task<Receipt> Park(string uri, double minutes, string spacePortName)
        {
            var response = await JsonPostRequestAsync(uri, new
            {
                minutes,
                spacePortName
            });
            var responseContentString = await response.Content.ReadAsStringAsync();
            var receipt = JsonConvert.DeserializeObject<Receipt>(responseContentString);
            return receipt;
        }
        public async Task<IEnumerable<SpacePort>> GetSpacePortsAsync(string uri)
        {
            var response = await JsonPostRequestAsync(uri, new
            {
            });
            var responseContentString = await response.Content.ReadAsStringAsync();
            var ports = JsonConvert.DeserializeObject<IEnumerable<SpacePort>>(responseContentString);
            return ports;
        }
        public async Task<double> GetPriceAsync(int spacePortId, string spaceShipModel, double minutes)
        {
            var query = QueryStringParse(new { spacePortId, spaceShipModel, minutes });
            var response = await GetRequestAsync(EndPoints.Parking.parkingPrice, query);
            var responseContentString = await response.Content.ReadAsStringAsync();
            var price = double.Parse(responseContentString);
            return price;
        }
        public async Task<IEnumerable<Receipt>> GetReceipts(string uri)
        {
            var response = await JsonPostRequestAsync(uri, new
            {
            });
            var responseContentString = await response.Content.ReadAsStringAsync();
            var receipts = JsonConvert.DeserializeObject<IEnumerable<Receipt>>(responseContentString);
            return receipts;
        }
        #endregion
    }
}

