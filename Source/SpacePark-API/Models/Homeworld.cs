using Newtonsoft.Json;

namespace SpacePark_API.Models
{
    public record Homeworld
    {
        public int HomeworldId { get; set; }
        public string Name { get; set; }

        [JsonProperty("Rotation_Period")] public string RotationPeriod { get; set; }

        [JsonProperty("Orbital_Period")] public string OrbitalPeriod { get; set; }

        public string Diameter { get; set; }
        public string Climate { get; set; }
        public string Terrain { get; set; }
        public string Population { get; set; }
        public string Url { get; set; }
    }
}