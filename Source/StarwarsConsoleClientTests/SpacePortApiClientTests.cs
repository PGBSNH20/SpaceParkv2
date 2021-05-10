using StarwarsConsoleClient.Networking;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace StarwarsConsoleClientTests
{
    public class SpacePortApiClientTests
    {
        private SpacePortApiClient Client;
        private bool disposedValue;

        public SpacePortApiClientTests()
        {
            Client = new SpacePortApiClient(@"https://localhost:5001/");
        }
        [Fact]
        public async Task LoginDarthMaul()
        {
            bool response = await Client.LoginAsync("BigRed", "Maul@123");
            Assert.True(response);
        }
        [Fact]
        public async Task Register()
        {
            bool response = await Client.RegisterAsync(
                "BTL Y-wing",
                "c-3po",
                "RoughBoy33",
                "NoToRust@123");
            Assert.True(response);
        }
        [Fact]
        public async Task RegisterHan()
        {
            bool response = await Client.RegisterAsync(
                "Millenium Falcon",
                "Han Solo",
                "BadMotherFucker",
                "Sparkels@123");
            Assert.True(response);
        }
    }
}
