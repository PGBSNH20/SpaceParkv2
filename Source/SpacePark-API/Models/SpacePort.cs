using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SpacePark_API.DataAccess;

namespace SpacePark_API.Models
{
    public record SpacePort
    {
        public SpacePort(string name, int parkingSpots, double priceMultiplier)
        {
            PriceMultiplier = priceMultiplier;
            ParkingSpots = parkingSpots;
            Name = name;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int ParkingSpots { get; private set; }
        public double PriceMultiplier { get; set; }
        public bool Enabled { get; set; }
        public string Park(Account account, double minutes, IStarwarsRepository repository)
        {
            if (GetActiveParkings(repository) < ParkingSpots)
            {
                var price = CalculatePrice(account.SpaceShip, minutes);
                var endTime = DateTime.Now.AddMinutes(minutes);
                var receipt = new Receipt
                {
                    Account = account,
                    Price = price,
                    StartTime = DateTime.Now.ToString("g"),
                    EndTime = endTime.ToString("g"),
                    SpacePort = this
                };
                repository.Update(receipt);
                repository.SaveChanges();
                return HttpStatusCode.OK.ToString();
            }
            else
            {
                return HttpStatusCode.Forbidden.ToString();
            }
        }
        private int GetActiveParkings(IStarwarsRepository repository)
        {
            var ongoingParkings = new List<Receipt>();
            foreach (var receipts in repository.Receipts)
            {
                if (receipts.SpacePort != this) continue;
                if (DateTime.Parse(receipts.EndTime) > DateTime.Now)
                {
                    ongoingParkings.Add(receipts);
                }
            }

          
                   
            return ongoingParkings.Count;
        }
        public decimal CalculatePrice(SpaceShip ship, double minutes)
        {
            var price = double.Parse(ship.ShipLength.Replace(".", ",")) * minutes / PriceMultiplier;
            return (decimal)price;
        }
    }


}
