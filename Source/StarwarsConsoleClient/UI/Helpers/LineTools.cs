using System;

namespace StarwarsConsoleClient.UI
{
    public static class LineTools
    {
        public static void ClearAt((int X, int Y) XY, string entered)
        {
            SetCursor(XY);
            foreach (var chr in entered)
                Console.Write(" ");
            SetCursor(XY);
        }

        public static void ClearAt((int X, int Y) XY, int count)
        {
            SetCursor(XY);
            for (var i = 0; i < count; i++)
                Console.Write(" ");
            SetCursor(XY);
        }

        public static void SetCursor((int X, int Y) XY)
        {
            Console.SetCursorPosition(XY.X, XY.Y);
        }
    }
}