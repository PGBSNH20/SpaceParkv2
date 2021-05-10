using System.ComponentModel.DataAnnotations;
using SpacePark_API.Models;

namespace SpacePark_API.Authentication
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Account name is required")]
        public string AccountName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        
        [Required(ErrorMessage = "Spaceship is required")]
        public string SpaceShipModel { get; set; }

    }
}
