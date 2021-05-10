using StarwarsConsoleClient.Networking;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace StarwarsConsoleClient.Main
{
    public static class TestMain
    {
        static TestMain()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            Client = new SpacePortApiClient(@"https://localhost:5001");
        }
        public static readonly SpacePortApiClient Client;
        public static (string accountName, string password) _namepass { get; set; }

        public static async Task TestMainMain(string[] args)
        {
            Console.WriteLine("Tap to login again!");
            Console.ReadKey();
            Console.WriteLine("Logging in...");
            await LoginDarthMaul();
            Console.WriteLine("---End of message---");
            Console.ReadLine();
        }
        private static async Task LoginDarthMaul()
        {
            await Client.LoginAsync("BigRed", "Maul@123");
        }
    }
}