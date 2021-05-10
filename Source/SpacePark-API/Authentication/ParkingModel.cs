using System.ComponentModel.DataAnnotations;

namespace SpacePark_API.Authentication
{
    public class ParkingModel
    {
        [Required(ErrorMessage = "Minutes is required")]
        public double Minutes { get; set; }
        [Required(ErrorMessage = "Spaceport-name is required")]
        public string SpacePortName { get; set; }
    }
}
