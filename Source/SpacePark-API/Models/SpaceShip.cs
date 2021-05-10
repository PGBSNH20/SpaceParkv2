using Newtonsoft.Json;

namespace SpacePark_API.Models
{
    public record SpaceShip
    {
        public int SpaceShipID { get; set; }
        public string Model { get; set; }
        public string Manufacturer { get; set; }

        [JsonProperty("cost_in_credits")] public string Price { get; set; }

        [JsonProperty("starship_class")] public string Classification { get; set; }

        [JsonProperty("Length")] public string ShipLength { get; set; }

        public string Url { get; set; }
        //Noticed SWAPI JSON doesn't contain values for height or width. Will we use our own measurements?
    }
}