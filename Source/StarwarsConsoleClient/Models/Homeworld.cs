using Newtonsoft.Json;

namespace StarwarsConsoleClient.Main
{
    public record Homeworld
    {
        public int HomeworldID { get; set; }
        public string Name { get; set; }
        public string RotationPeriod { get; set; }
        public string OrbitalPeriod { get; set; }
        public string Diameter { get; set; }
        public string Climate { get; set; }
        public string Terrain { get; set; }
        public string Population { get; set; }
        public string Url { get; set; }
    }
}