namespace StarwarsConsoleClient.Main
{
    public class Receipt
    {
        public int ReceiptID { get; set; }
        public Account Account { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public decimal Price { get; set; }
    }
}