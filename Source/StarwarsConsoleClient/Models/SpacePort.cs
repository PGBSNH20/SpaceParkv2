namespace StarwarsConsoleClient.Main
{
    public class SpacePort
    {
        public int id { get; set; }
        public string name { get; set; }
        public int parkingSpots { get; private set; }
        public double priceMultiplier { get; set; }
        public bool enabled { get; set; }
    }
}