using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SpacePark_API.Authentication;
using SpacePark_API.DataAccess;
using SpacePark_API.Models;
using SpacePark_API.Networking;

namespace SpacePark_API.Controllers
{
    public class AccountController : ControllerBase
    {
        private readonly IStarwarsRepository _repository;
        private readonly IConfiguration _configuration;
        public AccountController(IStarwarsRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }
        // GET: api/<UserController>
        [HttpPost]
        [Route("api/[controller]/Register")]
        public IActionResult Register([FromBody] RegisterModel model)
        {
            var person = APICollector.ParseUser(model.Name);
            var ship = APICollector.ParseShip(model.SpaceShipModel);
            if (person == null || ship == null)
            {
                return NotFound();
            }
            if (_repository.Accounts.FirstOrDefault(a => a.AccountName == model.AccountName) != null)
            {
                return Forbid();
            }
            ship = _repository.SpaceShips.FirstOrDefault(g => g.Model == ship.Model) ??
                   ship;
            var account = new Account
            {
                AccountName = model.AccountName,
                Password = model.Password,
                Person = person,
                SpaceShip = ship,
                Role = Role.User
            };
            person.Homeplanet = _repository.Homeworlds.FirstOrDefault(g => g.Name == person.Homeplanet.Name) ??
                                person.Homeplanet;
            _repository.Add(account);
            _repository.SaveChanges();

            return Ok(new { Status = "Success", Message = "User created successfully!" });
        }
        [HttpPost]
        [Route("api/[controller]/Login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
                var account = _repository.Accounts.FirstOrDefault(x => x.AccountName == model.Username);
            if (account == null) return Unauthorized();

             if (account.Password != model.Password || account.AccountName != model.Username) return Unauthorized();
            var identity = GetClaimsIdentity(account);
            var token = GetJwtToken(identity);
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            var userToken = new UserToken()
            {
                Account = account,
                ExpiryDate = token.ValidTo,
                Token = tokenString
            };
            _repository.Add(userToken);
            _repository.SaveChanges();
            return Ok(new
            {
                token = tokenString,
                expiration = token.ValidTo
            });
        }
        [Authorize]
        [HttpPost]
        [Route("api/[controller]/ChangeSpaceShip")]
        public IActionResult ChangeSpaceShip(string spaceshipModel)
        {
            var token = Request.Headers.FirstOrDefault(p => p.Key == "Authorization").Value.FirstOrDefault()?.Replace("Bearer ", "");
            var account = _repository.UserTokens.Where(a => a.Token == token)
                .Include(a => a.Account).Include(p => p.Account.Person).Include(ss => ss.Account.SpaceShip).SingleOrDefault()
                .Account;
            if (account == null)
                return NotFound("An account connected to this token was not found... strange...");
            var spaceship = APICollector.ParseShipAsync(spaceshipModel);
            if (spaceship == null)
                return NotFound($"A ship with the model {spaceshipModel} was not found.");
            account.SpaceShip = spaceship;
            _repository.Update(account);
            _repository.SaveChanges();
            return Ok(
                $"This account now uses spaceship {spaceship}"
            );
        }
        
        [Authorize]
        [HttpPost]
        [Route("api/[controller]/GetHomeworld")]
        public Homeworld GetHomeworld()
        {
            var token = Request.Headers.FirstOrDefault(p => p.Key == "Authorization").Value.FirstOrDefault()?.Replace("Bearer ", "");
            var homePlanet = _repository.UserTokens.Where(a => a.Token == token)
                .Include(a => a.Account).Include(hp => hp.Account.Person.Homeplanet).SingleOrDefault()
                .Account.Person.Homeplanet;
            return homePlanet;
        }
        [Authorize]
        [HttpPost]
        [Route("api/[controller]/MySpaceShip")]
        public SpaceShip MySpaceShip()
        {
            var token = Request.Headers.FirstOrDefault(p => p.Key == "Authorization").Value.FirstOrDefault()?.Replace("Bearer ", "");
            var spaceship = _repository.UserTokens.Where(a => a.Token == token)
                .Include(a => a.Account).Include(hp => hp.Account.SpaceShip).SingleOrDefault()
                .Account.SpaceShip;
            return spaceship;
        }
        [Authorize]
        [HttpPost]
        [Route("api/[controller]/MyData")]
        public Person MyData()
        {
            var token = Request.Headers.FirstOrDefault(p => p.Key == "Authorization").Value.FirstOrDefault()?.Replace("Bearer ", "");
            var person = _repository.UserTokens.Where(a => a.Token == token)
                .Include(a => a.Account).Include(hp => hp.Account.Person).SingleOrDefault()
                .Account.Person;
            return person;
        }
        [Authorize]
        [HttpPost]
        [Route("api/[controller]/MyReceipts")]
        public List<Receipt> GetReceipts()
        {
            var token = Request.Headers.FirstOrDefault(p => p.Key == "Authorization").Value.FirstOrDefault()?.Replace("Bearer ", "");
            var account = _repository.UserTokens.Where(a => a.Token == token)
                .Include(a => a.Account).SingleOrDefault()
                .Account;
            var receipts = _repository.Receipts.Where(r => r.Account == account)
                .Include(a => a.Account)
                .Include(p => p.Account.Person)
                .Include(sp => sp.SpacePort)
                .Include(ss => ss.Account.SpaceShip).ToList();
            return receipts;
        }

        [HttpGet]
        [Route("api/[controller]/Ships")]
        public List<SpaceShip> Get(int maxLength = 150)
        {
            var ships = APICollector.ReturnShipsAsync().Where(s => double.Parse(s.ShipLength.Replace(".", ",")) <= maxLength).ToList();
            return ships;
        }
        private JwtSecurityToken GetJwtToken(ClaimsIdentity identity)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                notBefore: DateTime.UtcNow,
                claims: identity.Claims,
                // our token will live 1 hour, but you can change you token lifetime here
                expires: DateTime.UtcNow.Add(TimeSpan.FromHours(3)),
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));
            return jwtSecurityToken;
        }
        private ClaimsIdentity GetClaimsIdentity(Account account)
        {
            // Here we can save some values to token.
            // For example we are storing here user id and email
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, account.AccountName)
            };
            var claimsIdentity = new ClaimsIdentity(claims, "Token");

            // Adding roles code
            // Roles property is string collection but you can modify Select code if it it's not
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, account.Role));
            return claimsIdentity;
        }
   
    }
}