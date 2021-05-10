using SpacePark_API.Models;
using System.Linq;

namespace SpacePark_API.DataAccess
{
    public interface IStarwarsRepository
    {
        IQueryable<Account> Accounts { get; }
        IQueryable<Person> People { get; }
        IQueryable<Receipt> Receipts { get; }
        IQueryable<SpaceShip> SpaceShips { get; }
        IQueryable<Homeworld> Homeworlds { get; }
        IQueryable<SpacePort> SpacePorts { get; }
        IQueryable<UserToken> UserTokens { get; }
        void Add<TEntityType>(TEntityType entity);
        void Update<TEntityType>(TEntityType entity);
        void Remove<TEntityType>(TEntityType entity);
        void SaveChanges();
    }
}