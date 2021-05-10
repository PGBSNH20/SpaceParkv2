namespace SpacePark_API.Authentication
{
    public class AddSpacePortModel
    {
        public string Name { get; set; }
        public int ParkingSpots { get; set; }
        public int PriceMultiplier { get; set; }
    }
    public class ChangeSpacePortAvailability
    {
        public string Name { get; set; }
        public bool Enabled { get; set; }
    }
    public class UpdateSpacePortPrice
    {
        public string Name { get; set; }
        public double SpacePortMultiplier { get; set; }
    }
}