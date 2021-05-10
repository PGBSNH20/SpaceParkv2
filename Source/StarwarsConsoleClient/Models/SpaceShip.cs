using Newtonsoft.Json;

namespace StarwarsConsoleClient.Main
{
    public class SpaceShip
    {
        public int SpaceShipID { get; set; }
        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public string Price { get; set; }
        public string Classification { get; set; }
        public string ShipLength { get; set; }
        public string Url { get; set; }
    }
}