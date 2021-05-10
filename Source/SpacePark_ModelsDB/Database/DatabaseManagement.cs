//using Microsoft.EntityFrameworkCore;
//using SpacePark_API.Models;
//using SpacePark_API.Networking;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Threading;

//namespace SpacePark_API.DataAccess
//{
//    public partial class DatabaseManagement
//    {
//        public static double PriceMultiplier = 10;
//        public static int ParkingSlots = 5;
//        private static string ConnectionString { get; set; }

//        public static void SetConnectionString(string filePath) { ConnectionString = File.ReadAllText(filePath); }

//        public class ParkingManagement
//        {
//            private ParkingManagement() //Instantiation. This will help us throw if ConnectionString is null.
//            {
//                if (ConnectionString == null)
//                    throw new Exception("The static property ConnectionString has not been assigned.",
//                        new Exception(
//                            "Please assign a value to the static property ConnectionString before calling any methods"));
//            }

//            #region Static Methods

//            //These methods instantiate ParkingManagement and call upon the private non-static methods.
//            public static (bool isOpen, DateTime nextAvailable) CheckParkingStatus()
//            {
//                var pm = new ParkingManagement();
//                return pm._CheckParkingStatus();
//            }

//            public static decimal CalculatePrice(SpaceShip ship, double minutes)
//            {
//                var pm = new ParkingManagement();
//                return pm._CalculatePrice(ship, minutes);
//            }

//            public static Receipt SendInvoice(Account account, double minutes)
//            {
//                var pm = new ParkingManagement();
//                return pm._SendInvoice(account, minutes);
//            }

//            #endregion

//            #region Instantiated Methods

//            private (bool isOpen, DateTime nextAvailable) _CheckParkingStatus()
//            {
//                var dbHandler = new StarwarsContext { ConnectionString = ConnectionString };
//                var ongoingParkings = new List<Receipt>();
//                foreach (var receipts in dbHandler.Receipts)
//                    if (DateTime.Parse(receipts.EndTime) > DateTime.Now)
//                        ongoingParkings.Add(receipts);
//                var nextAvailable = DateTime.Now;
//                var isOpen = false;
//                if (ongoingParkings.Count >= ParkingSlots)
//                {
//                    //Setting nextAvailable 10 years ahead so the loop will always start running.
//                    nextAvailable = DateTime.Now.AddYears(10);
//                    var cachedNow = DateTime.Now;
//                    //Caching DateTimeNow in case loops takes longer than expected, to ensure that time moving forward doesn't break the loop.
//                    foreach (var receipt in ongoingParkings)
//                    {
//                        var endTime = DateTime.Parse(receipt.EndTime);
//                        if (endTime > cachedNow && endTime < nextAvailable) nextAvailable = endTime;
//                    }
//                }
//                else
//                {
//                    isOpen = true;
//                }

//                return (isOpen, nextAvailable);
//            }

//            private decimal _CalculatePrice(SpaceShip ship, double minutes)
//            {
//                var price = double.Parse(ship.ShipLength.Replace(".", ",")) * minutes / PriceMultiplier;
//                return (decimal)price;
//            }

//            private Receipt _SendInvoice(Account account, double minutes)
//            {
//                var receipt = new Receipt();
//                var thread = new Thread(() => { receipt = Execution_SendInvoice(account, minutes); });
//                thread.Start();
//                thread.Join(); //By doing join it will wait for the method to finish
//                return receipt;
//            }

//            private Receipt Execution_SendInvoice(Account account, double minutes)
//            {
//                var price = _CalculatePrice(account.SpaceShip, minutes);
//                var endTime = DateTime.Now.AddMinutes(minutes);
//                var receipt = new Receipt
//                {
//                    Account = account,
//                    Price = price,
//                    StartTime = DateTime.Now.ToString("g"),
//                    EndTime = endTime.ToString("g")
//                };
//                if (ConnectionString == null)
//                    throw new Exception("The static property ConnectionString has not been assigned.",
//                        new Exception(
//                            "Please assign a value to the static property ConnectionString before calling any methods"));
//                var dbHandler = new StarwarsContext { ConnectionString = ConnectionString };
//                dbHandler.Receipts.Update(receipt);
//                dbHandler.SaveChanges(); //TODO
//                return receipt;
//            }

//            #endregion
//        }

//        public partial class AccountManagement
//        {
//            private AccountManagement()
//            {
//                if (ConnectionString == null)
//                    throw new Exception("The static property ConnectionString has not been assigned.",
//                        new Exception(
//                            "Please assign a value to the static property ConnectionString before calling any methods"));
//            } //Instantiation. This will help us throw if ConnectionString is null.
//            #region Instantiated Methods

//            #region Overloads

//            private bool _Exists(Person inputPerson)
//            {
//                var result = false;
//                var thread = new Thread(() => { result = Execution_Exists(inputPerson); });
//                thread.Start();
//                thread.Join(); //By doing join it will wait for the method to finish
//                return result;
//            }

//            private List<Receipt> GetAccountReceipts(string accountName)
//            {
//                var receiptList = new List<Receipt>();
//                var dbHandler = new StarwarsContext { ConnectionString = ConnectionString };
//                foreach (var receipt in dbHandler.Receipts)
//                    if (receipt.Account.AccountName == accountName)
//                        receiptList.Add(receipt);
//                return receiptList;
//            }

//            private List<Receipt> GetAccountReceipts(int accountId)
//            {
//                var receiptList = new List<Receipt>();
//                var dbHandler = new StarwarsContext { ConnectionString = ConnectionString };
//                foreach (var receipt in dbHandler.Receipts)
//                    if (receipt.Account.AccountID == accountId)
//                        receiptList.Add(receipt);
//                return receiptList;
//            }

//            private bool Execution_Exists(Person inputPerson)
//            {
//                var result = false;
//                var dbHandler = new StarwarsContext { ConnectionString = ConnectionString };
//                foreach (var p in dbHandler.Persons)
//                    if (p.Name == inputPerson.Name)
//                        result = true;

//                return result;
//            }

//            #endregion

//            //Async method. Below will call upon a private corresponding method in another thread.
//            private void _Register(Person inputPerson, SpaceShip inputShip, string accountName, string password)
//            {
//                var dbHandler = new StarwarsContext { ConnectionString = ConnectionString };
//                var outputAccount = new Account
//                {
//                    AccountName = accountName,
//                    Password = PasswordHashing.HashPassword(password),
//                    Person = inputPerson,
//                    SpaceShip = inputShip
//                };
//                inputPerson.Homeplanet = dbHandler.Homeworlds.FirstOrDefault(g => g.Name == inputPerson.Homeplanet.Name) ??
//                                       inputPerson.Homeplanet;
//                dbHandler.Accounts.Add(outputAccount);
//                dbHandler.SaveChanges();
//            }

//            private List<Receipt> _GetAccountReceipts(Account account)
//            {
//                var dbHandler = new StarwarsContext { ConnectionString = ConnectionString };
//                return dbHandler.Receipts.Include(a => a.Account)
//                    .Where(receipt => receipt.Account.AccountName == account.AccountName).ToList();
//            }

//            //Async method. Below will call upon a private corresponding method in another thread.
//            private bool _Exists(string name, bool isAccountName)
//            {
//                var result = false;
//                var thread = new Thread(() => { result = Execution_Exists(name, isAccountName); });
//                thread.Start();
//                thread.Join(); //By doing join it will wait for the method to finish
//                return result;
//            }

//            //Execution method that will do the "work"
//            private bool Execution_Exists(string name, bool isAccountName)
//            {
//                if (isAccountName)
//                {
//                    var result = false;
//                    var dbHandler = new StarwarsContext { ConnectionString = ConnectionString };
//                    foreach (var account in dbHandler.Accounts)
//                        if (account.AccountName == name)
//                            result = true;

//                    return result;
//                }
//                else
//                {
//                    var result = false;
//                    var dbHandler = new StarwarsContext { ConnectionString = ConnectionString };
//                    foreach (var account in dbHandler.Persons)
//                        if (account.Name == name)
//                            result = true;

//                    return result;
//                }
//            }

//            private Account _ValidateLogin(string accountName, string passwordInput)
//            {
//                var account = new Account();
//                var thread = new Thread(() =>
//                {
//                    account = Execution_ValidateLogin(accountName, PasswordHashing.HashPassword(passwordInput));
//                });
//                thread.Start();
//                thread.Join(); //By doing join it will wait for the method to finish
//                return account;
//            }

//            private Account Execution_ValidateLogin(string accountName, string passwordInput)
//            {
//                Account accountHolder = null;
//                var dbHandler = new StarwarsContext { ConnectionString = ConnectionString };
//                foreach (var account in dbHandler.Accounts.Include(a => a.Person).Include(a => a.SpaceShip)
//                    .Include(h => h.Person.Homeplanet))
//                    if (account.AccountName == accountName && account.Password == passwordInput)
//                        accountHolder = account;
//                return accountHolder;
//            }

//            #endregion

//            #region Static Methods

//            #region Private Methods

//            private static (string question, string answer) GetSecurityQuestion(Person inputPerson)
//            {
//                var question = string.Empty;
//                var answer = string.Empty;
//                var r = new Random();
//                var x = r.Next(1, 4);
//                switch (x)
//                {
//                    case 1:
//                        question = "What is your hair color?";
//                        answer = inputPerson.HairColor;
//                        break;
//                    case 2:
//                        question = "What is your skin color?";
//                        answer = inputPerson.SkinColor;
//                        break;
//                    case 3:
//                        question = "What is your eye color?";
//                        answer = inputPerson.EyeColor;
//                        break;
//                    case 4:
//                        question = "What is your birth year?";
//                        answer = inputPerson.BirthYear;
//                        break;
//                }

//                return (question, answer);
//            }

//            #endregion

//            #region Public Methods & IEnumerables

//            //These methods instantiate AccountManagement and call upon the private non-static methods.
//            public static IEnumerable<Receipt> GetAccountReceipts(Account account)
//            {
//                var am = new AccountManagement();
//                return am._GetAccountReceipts(account);
//            }

//            public static Account ValidateLogin(string accountName, string passwordInput)
//            {
//                var am = new AccountManagement();
//                return am._ValidateLogin(accountName, passwordInput);
//            }

//            public static void Register(Person inputPerson, SpaceShip inputShip, string accountName, string password)
//            {
//                var am = new AccountManagement();
//                am._Register(inputPerson, inputShip, accountName, password);
//            }

//            public static void ReRegisterShip(Account account, SpaceShip ship)
//            {
//                ship.SpaceShipID = account.SpaceShip.SpaceShipID;
//                account.SpaceShip = ship;

//                var dbHandler = new StarwarsContext { ConnectionString = ConnectionString };
//                dbHandler.SpaceShips.Update(ship);
//                dbHandler.SaveChanges();
//            }

//            public static Person IdentifyWithQuestion(string name, Func<string, string> getSecurityAnswer)
//            {
//                var inputPerson = APICollector.ParsePersonAsync(name);
//                if (inputPerson == null) return null;

//                var (question, answer) = GetSecurityQuestion(inputPerson);
//                var inputAnswer = getSecurityAnswer(question);
//                if (inputAnswer.ToLower() == answer.ToLower()) return inputPerson;
//                return null;
//            }

//            public static bool Exists(string name, bool isAccountName)
//            {
//                var am = new AccountManagement();
//                return am._Exists(name, isAccountName);
//            }
            
//            #region Overloads
//            public static bool Exists(Person person)
//            {
//                var am = new AccountManagement();
//                return am._Exists(person);
//            }

//#endregion
//#endregion
//#endregion

       
//        }
//    }
//}