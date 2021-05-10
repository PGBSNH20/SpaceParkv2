using StarwarsConsoleClient.Main;
using System;

namespace StarwarsConsoleClient.UI
{
    public class TimeGetter
    {
        public TimeGetter((int X, int Y) hourXY, (int X, int Y) priceXY, int maxValue,
            Func<double, double> calculate)
        {
            HourXY = hourXY;
            PriceXY = priceXY;
            MaxValue = maxValue;
            Calculate = calculate;
        }

        private (int X, int Y) HourXY { get; }
        private (int X, int Y) PriceXY { get; }
        private int MaxValue { get; }
        public decimal Price { get; set; }
        public double HoursSelected { get; set; }
        private Func<double, double> Calculate { get; }

        public double GetMinutes(SpaceShip ship)
        {
            Console.CursorVisible = false;
            Console.ForegroundColor = ConsoleColor.Green;
            var hoursSelected = HoursSelected;
            var price = Price;

            var priceData = new LineData(PriceXY);
            var hourData = new LineData(HourXY);
            priceData.Update(price.ToString());
            hourData.Update(HoursSelected.ToString());

            var keyInfo = Console.ReadKey(true);

            while (keyInfo.Key != ConsoleKey.Enter)
            {
                if (keyInfo.Key == ConsoleKey.UpArrow && hoursSelected <= MaxValue)
                {
                    hoursSelected++;
                    price = Math.Round((decimal)Calculate(hoursSelected), 2);
                    Console.CursorVisible = false;
                    priceData.Update((price * 60).ToString());
                    Console.CursorVisible = true;
                    hourData.Update(hoursSelected.ToString());
                }

                if (keyInfo.Key == ConsoleKey.DownArrow && hoursSelected != 0)
                {
                    hoursSelected--;
                    price = Math.Round((decimal)Calculate(hoursSelected), 2);
                    Console.CursorVisible = false;
                    priceData.Update((price * 60).ToString());
                    Console.CursorVisible = true;
                    hourData.Update(hoursSelected.ToString());
                }

                keyInfo = Console.ReadKey(true);
            }

            Price = price;
            HoursSelected = hoursSelected;

            return hoursSelected * 60;
        }
    }
}