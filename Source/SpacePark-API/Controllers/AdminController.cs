﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpacePark_API.Authentication;
using SpacePark_API.DataAccess;
using SpacePark_API.Models;

namespace SpacePark_API.Controllers
{
    public class AdminController : ControllerBase
    {
        private readonly IStarwarsRepository _repository;
        public AdminController(IStarwarsRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [Route("api/[controller]/Account")]
        public List<Account> Get()
        {
            var accounts = _repository.Accounts
                .Include(p => p.Person)
                .Include(ss => ss.SpaceShip)
                .Include(hw => hw.Person.Homeplanet).ToList();
            return accounts;
        }
        
        [HttpPost]
        [Route("api/[controller]/AddSpacePort")]
        public IActionResult Get([FromBody] AddSpacePortModel model)
        {
            if (_repository.SpacePorts.Any() && _repository.SpacePorts.Single(p => p.Name == model.Name) != null)
                return Forbid();


            var port = new SpacePort(model.Name, model.ParkingSpots, model.PriceMultiplier) {Enabled = true };
                                                                                            //Mark the port as enabled so we can park.
            _repository.Add(port);
            _repository.SaveChanges();

            return Ok(new {
                Name = port.Name, 
                ParkingSpots = model.ParkingSpots,
                PriceMultiplier = model.PriceMultiplier,
                
            });
        }

        [HttpPost]
        [Route("api/[controller]/SpacePortEnabled")]
        public IActionResult Get([FromBody] ChangeSpacePortAvailabilityId model)
        {
            if (!_repository.SpacePorts.Any() && _repository.SpacePorts.Single(p => p.Id == model.SpacePortId) == null)
                return NotFound();
            var port = _repository.SpacePorts.Single(p => p.Id == model.SpacePortId);
            port.Enabled = false;
            _repository.Update(port);
            _repository.SaveChanges();
            return Ok(
                $"SpacePort with ID: {model.SpacePortId} has been set to {model.Enabled}"
            );
        }
        private void SpacePort_ParseIdName(string input)
        {
            
        }

        [HttpPost]
        [Route("api/[controller]/DeleteSpacePort")]
        public IActionResult Get([FromBody] RemoveSpacePortModelId model)
        {
            if (!_repository.SpacePorts.Any() && _repository.SpacePorts.Single(p => p.Id == model.SpacePortId) == null)
                return NotFound();
            var port = _repository.SpacePorts.Single(p => p.Id == model.SpacePortId);
            _repository.Remove(port);
            _repository.SaveChanges();
            return Ok(
                $"SpacePort with ID: {model.SpacePortId} has been removed"
            );
        }
        
        [HttpPost]
        [Route("api/[controller]/UpdateSpacePortPrice")]
        public IActionResult Get([FromBody] UpdateSpacePortPriceId model)
        {
            if (!_repository.SpacePorts.Any() && _repository.SpacePorts.Single(p => p.Id == model.SpacePortId) == null)
                return NotFound();
            var port = _repository.SpacePorts.Single(p => p.Id == model.SpacePortId);
            var oldMultiplier = port.PriceMultiplier; //Cache old mult. for message
            port.PriceMultiplier = model.SpacePortMultiplier;
            _repository.Update(port);
            _repository.SaveChanges();
            return Ok(
                $"SpacePort with ID: {model.SpacePortId} has been updated with the new price multiplier to {model.SpacePortMultiplier} from {oldMultiplier}"
            );
        }
        
        [HttpPost]
        [Route("api/[controller]/UpdateSpacePortName")]
        public IActionResult Get([FromBody] UpdateSpacePortNameId model)
        {
            if (!_repository.SpacePorts.Any() && _repository.SpacePorts.Single(p => p.Id == model.SpacePortId) == null)
                return NotFound();
            if (_repository.SpacePorts.Single(p => p.Name == model.NewSpacePortName) != null)
                return Forbid("Name is already taken");
            var port = _repository.SpacePorts.Single(p => p.Id == model.SpacePortId);
            var oldName = port.Name; //Cache old name. for message
            port.Name = model.NewSpacePortName;
            _repository.Update(port);
            _repository.SaveChanges();
            return Ok(
                $"SpacePort with ID: {model.SpacePortId} has gained a new name. From {oldName} to {model.NewSpacePortName}"
            );
        }

    }
}