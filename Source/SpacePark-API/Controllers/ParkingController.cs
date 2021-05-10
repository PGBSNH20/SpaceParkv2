using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpacePark_API.Authentication;
using SpacePark_API.DataAccess;
using SpacePark_API.Models;
using SpacePark_API.Networking;

namespace SpacePark_API.Controllers
{
    
    [ApiController]
    public class ParkingController : ControllerBase
    {
        private readonly IStarwarsRepository _repository;

        public ParkingController(IStarwarsRepository repository)
        {
            _repository = repository;
        }
        [Authorize]
        [HttpPost]
        [Route("api/[controller]/Park")]
        public IActionResult Park([FromBody] ParkingModel model)
        {
            var spacePort = _repository.SpacePorts.SingleOrDefault(sp => sp.Name == model.SpacePortName);
            var token = Request.Headers.FirstOrDefault(p => p.Key == "Authorization").Value.FirstOrDefault()?.Replace("Bearer ", "");
            var account = _repository.UserTokens.Where(a => a.Token == token)
                .Include(a => a.Account).Include(p => p.Account.Person).Include(ss => ss.Account.SpaceShip).SingleOrDefault()
                .Account;
            if (account == null || spacePort == null)
                return NotFound();
            if (!checkParkingStatus(spacePort).isOpen)
                return Conflict($"The Spaceport is currently full and will be available in");

            var price = spacePort.CalculatePrice(account.SpaceShip, model.Minutes);
            var receipt = new Receipt
            {
                Price = price,
                StartTime = DateTime.Now.ToString("g"),
                EndTime = DateTime.Now.AddMinutes(model.Minutes).ToString("g"),
                SpacePort = spacePort,
                Account = account
            };
            _repository.Add(receipt);
            _repository.SaveChanges();

            return Ok(receipt);
        }
       
        [HttpGet]
        [Route("/api/[controller]/IsAvailable")]
        public IActionResult IsOpen(string spacePortName)
        {
            var spacePort = _repository.SpacePorts.SingleOrDefault(sp => sp.Name == spacePortName);
            if (spacePort == null)
                return NotFound($"Spaceport with the name {spacePortName} was not found.");

            if (!spacePort.Enabled)
                return Conflict($"Spaceport with the name {spacePortName} is closed.");
            var checkAvailability = checkParkingStatus(spacePort);
            return Ok(checkAvailability);
        }
        [Authorize]
        [HttpPost]
        [Route("api/[controller]/GetSpacePorts")]
        public List<SpacePort> GetSpacePorts()
        {
            return _repository.SpacePorts.ToList();
        }
        
        [Authorize]
        [HttpGet]
        [Route("api/[controller]/Price")]
        public decimal GetPrice(string spacePortName, string spaceShipModel, double minutes)
        {
            var spacePort = _repository.SpacePorts.SingleOrDefault(sp => sp.Name == spacePortName);
            var spaceShip = APICollector.ParseShipAsync(spaceShipModel);
            return spacePort.CalculatePrice(spaceShip, minutes);
        }
        [HttpGet]
        [Route("api/[controller]/ParkingStatus")]
        private (bool isOpen, DateTime nextAvailable) checkParkingStatus(SpacePort spacePort)
        {

            var ongoingParkings = new List<Receipt>();
            foreach (var receipts in _repository.Receipts.Include(r => r.SpacePort).Where(r => r.SpacePort.Name == spacePort.Name))
                if (DateTime.Parse(receipts.EndTime) > DateTime.Now)
                    ongoingParkings.Add(receipts);
            var nextAvailable = DateTime.Now;
            var isOpen = false;
            if (ongoingParkings.Count >= spacePort.ParkingSpots)
            {
                //Setting nextAvailable 10 years ahead so the loop will always start running.
                nextAvailable = DateTime.Now.AddYears(10);
                var cachedNow = DateTime.Now;
                //Caching DateTimeNow in case loops takes longer than expected, to ensure that time moving forward doesn't break the loop.
                foreach (var receipt in ongoingParkings)
                {
                    var endTime = DateTime.Parse(receipt.EndTime);
                    if (endTime > cachedNow && endTime < nextAvailable) nextAvailable = endTime;
                }
            }
            else
            {
                isOpen = true;
            }
            return (isOpen, nextAvailable);
        }

    }
}