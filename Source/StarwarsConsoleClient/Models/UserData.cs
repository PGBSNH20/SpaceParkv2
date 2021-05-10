namespace StarwarsConsoleClient.Main
{
    public static class UserData
    {
        public static Person Person { get; set; }
        public static SpaceShip SpaceShip { get; set; }
        public static SpacePort SelectedSpacePort { get; set; }
        public static string AccountName { get; set; }
        public static string tryFullName { get; set; }
        public static string Password { get; set; }
        public static readonly string BaseAPIUrl = "https://localhost:5001";
        public static void ClearData()
        {
            Person = null;
            SpaceShip = null;
            AccountName = null;
            Password = null;
        }
    }
}