using System;

namespace StarwarsConsoleClient.UI
{
    public class LineData
    {
        public LineData((int X, int Y) xy)
        {
            XY = xy;
        }
        private string LastWritten { get; set; } = "";
        private (int X, int Y) XY { get; }

        public void Update(string newString)
        {
            LineTools.SetCursor(XY);
            if (LastWritten.Length > newString.Length)
            {
                Console.Write(newString);
                for (var i = 0; i < LastWritten.Length - newString.Length; i++)
                    Console.Write(" ");
            }
            else
            {
                Console.Write(newString);
            }

            LastWritten = newString;
        }
    }
}