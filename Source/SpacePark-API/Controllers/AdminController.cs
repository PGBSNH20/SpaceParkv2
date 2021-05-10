using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
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
        [AuthorizeRoles(Role.Administrator)]
        [HttpPost]
        [Route("api/[controller]/Accounts")]
        public List<Account> ListAccounts()
        {
            var accounts = _repository.Accounts
                .Include(p => p.Person)
                .Include(ss => ss.SpaceShip)
                .Include(hw => hw.Person.Homeplanet).ToList();
            return accounts;
        }
        [AuthorizeRoles(Role.Administrator)]
        [HttpPost]
        [Route("api/[controller]/GetAccount")]
        public List<Account> GetAccount(string name)
        {
            var accounts = _repository.Accounts
                .Where(a => a.AccountName == name)
                .Include(p => p.Person)
                .Include(ss => ss.SpaceShip)
                .Include(hw => hw.Person.Homeplanet).ToList();
            return accounts;
        }
        [AuthorizeRoles(Role.Administrator)]
        [HttpPost]
        [Route("api/[controller]/AddSpacePort")]
        public IActionResult Post([FromBody] AddSpacePortModel model)
        {
            if (_repository.SpacePorts.Any() && _repository.SpacePorts.SingleOrDefault(p => p.Name == model.Name) != null)
                return Conflict($"Spaceport with name {model.Name} already exists");


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
        [AuthorizeRoles(Role.Administrator)]
        [HttpPost]
        [Route("api/[controller]/SpacePortEnabled")]
        public IActionResult Post([FromBody] ChangeSpacePortAvailability model)
        {
            if (!_repository.SpacePorts.Any() && _repository.SpacePorts.SingleOrDefault(p => p.Name == model.Name) == null)
                return NotFound($"Spaceport with name {model.Name} was not found.");
            var port = _repository.SpacePorts.SingleOrDefault(p => p.Name == model.Name);
            port.Enabled = model.Enabled;
            _repository.Update(port);
            _repository.SaveChanges();
            return Ok(
                $"SpacePort with the name: {model.Name} has been set to {model.Enabled}"
            );
        }
        [AuthorizeRoles(Role.Administrator)]
        [HttpPost]
        [Route("api/[controller]/DeleteSpacePort")]
        public IActionResult RemoveSpacePort(string spacePortName)
        {
            if (!_repository.SpacePorts.Any() && _repository.SpacePorts.SingleOrDefault(p => p.Name == spacePortName) == null)
                return NotFound($"Spaceport with name {spacePortName} was not found.");
            var port = _repository.SpacePorts.SingleOrDefault(p => p.Name == spacePortName);
            _repository.Remove(port);
            _repository.SaveChanges();
            return Ok(
                $"SpacePort with the name: {spacePortName} has been removed"
            );
        }
        [AuthorizeRoles(Role.Administrator)]
        [HttpPost]
        [Route("api/[controller]/UpdateSpacePortPrice")]
        public IActionResult UpdateSpacePortPrice([FromBody] UpdateSpacePortPrice model)
        {
            if (!_repository.SpacePorts.Any() && _repository.SpacePorts.SingleOrDefault(p => p.Name == model.Name) == null)
                return NotFound($"Spaceport with name {model.Name} was not found.");
            var port = _repository.SpacePorts.SingleOrDefault(p => p.Name == model.Name);
            var oldMultiplier = port.PriceMultiplier; //Cache old mult. for message
            port.PriceMultiplier = model.SpacePortMultiplier;
            _repository.Update(port);
            _repository.SaveChanges();
            return Ok(
                $"SpacePort with the name: {model.Name} has been updated with the new price multiplier to {model.SpacePortMultiplier} from {oldMultiplier}"
            );
        }
        [AuthorizeRoles(Role.Administrator)]
        [HttpPost]
        [Route("api/[controller]/UpdateSpacePortName")]
        public IActionResult UpdateSpacePortName(string oldSpacePortName, string newSpacePortName)
        {
            if (!_repository.SpacePorts.Any())
                return NotFound("There are no registered Spaceports.");
            if (_repository.SpacePorts.SingleOrDefault(p => p.Name == newSpacePortName) != null)
                return Conflict($"Spaceport with name {newSpacePortName} already exists");
            var port = _repository.SpacePorts.SingleOrDefault(p => p.Name == oldSpacePortName);
            port.Name = newSpacePortName;
            _repository.Update(port);
            _repository.SaveChanges();
            return Ok(
                $"SpacePort with the previous name of {oldSpacePortName} has gained a new name; {newSpacePortName}"
            );
        }
        [AuthorizeRoles(Role.Administrator)]
        [HttpPost]
        [Route("api/[controller]/PromoteAdmin")]
        public IActionResult PromoteAdmin(string accountName)
        {
            if (!_repository.Accounts.Any())
                return NotFound("There are no registered accounts.");
            if (_repository.Accounts.SingleOrDefault(a => a.AccountName == accountName) == null)
                return NotFound($"Account with name {accountName} was not found.");
            
            var account = _repository.Accounts.SingleOrDefault(a => a.AccountName == accountName);
            account.Role = Role.Administrator;
            _repository.Update(account);
            _repository.SaveChanges();
            return Ok(
                $"Account with the name of {accountName} has been promoted to administrator"
            );
        }
        [AuthorizeRoles(Role.Administrator)]
        [HttpPost]
        [Route("api/[controller]/DemoteAdmin")]
        public IActionResult DemoteAdmin(string accountName)
        {
            if (!_repository.Accounts.Any())
                return NotFound("There are no registered accounts.");
            if (_repository.Accounts.SingleOrDefault(a => a.AccountName == accountName) == null)
                return NotFound($"Account with name {accountName} was not found.");
            
            var account = _repository.Accounts.SingleOrDefault(a => a.AccountName == accountName);
            account.Role = Role.User;
            _repository.Update(account);
            _repository.SaveChanges();
            return Ok(
                $"Account with the name of {accountName} has been demoted to user"
            );
        }
    }
}