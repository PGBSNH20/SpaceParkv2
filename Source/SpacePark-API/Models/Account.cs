using System.Collections.Generic;
using System.Collections.Specialized;
using SpacePark_API.Authentication;

namespace SpacePark_API.Models
{
    public record Account
    {
        public int AccountId { get; set; }
        public Person Person { get; set; }
        public SpaceShip SpaceShip { get; set; }
        public string AccountName { get; set; }
        public string Password { get; set; }

        public string Role { get; set; }
    }
}