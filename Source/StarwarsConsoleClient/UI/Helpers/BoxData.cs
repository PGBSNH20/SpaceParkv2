using System;

namespace StarwarsConsoleClient.UI
{
    public class BoxData
    {
        public BoxData((int X, int Y) xy)
        {
            XY = xy;
        }
        private string[] LastWritten { get; set; } = new string[] { "" };
        private (int X, int Y) XY { get; }

        public void Update(string[] newStrings)
        {
            int i = 0;
            bool maxNew = false;
            bool maxLast = false;

            while(!maxNew || !maxLast)
            {
                maxNew = newStrings.Length <= i;
                maxLast = LastWritten.Length <= i;

                int lastChars = maxLast == true ? 0 : LastWritten[i].Length;
                int newChars = maxNew == true ? 0 : newStrings[i].Length;
                int whiteSpaces = lastChars > newChars ? lastChars - newChars : 0;

                LineTools.SetCursor((XY.X, XY.Y + i));

                if(maxNew == false)
                    Console.Write(newStrings[i]);

                for (int j = 0; j < whiteSpaces; j++)
                    Console.Write(" ");

                i++;
            }
        }
    }
}