using System;

namespace StarwarsConsoleClient.UI
{
    public class InputLine
    {
        public InputLine(IDrawable drawable, int maxChars, ConsoleColor foregroundColor)
        {
            X = drawable.CoordinateX + 2;
            Y = drawable.CoordinateY;
            MaxChars = maxChars;
            ForegroundColor = foregroundColor;
        }

        public InputLine(int x, int y, int maxChars, ConsoleColor foregroundColor)
        {
            X = x;
            Y = y;
            MaxChars = maxChars;
            ForegroundColor = foregroundColor;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int MaxChars { get; set; }
        private string InputString { get; set; }
        private ConsoleColor ForegroundColor { get; }

        public string GetInputString(bool isPassword)
        {
            Console.SetCursorPosition(X, Y);
            Console.CursorVisible = true;
            Console.ForegroundColor = ForegroundColor;
            var inputString = string.Empty;
            var keyInfo = Console.ReadKey(true);

            while (keyInfo.Key != ConsoleKey.Enter)
            {
                if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (inputString.Length > 0)
                    {
                        inputString = inputString.Remove(inputString.Length - 1);
                        Console.CursorLeft--;
                        Console.Write(" ");
                        Console.CursorLeft--;
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Tab || keyInfo.Key == ConsoleKey.Spacebar)
                {
                }
                else if (inputString.Length < MaxChars)
                {
                    inputString += keyInfo.KeyChar;
                    if (isPassword)
                        Console.Write("*");
                    else
                        Console.Write(keyInfo.KeyChar);
                }

                keyInfo = Console.ReadKey(true);
            }

            if (inputString.Length == 0)
                return GetInputString(isPassword);

            Console.CursorVisible = false;
            return inputString;
        }

        public void Clear()
        {
            Console.SetCursorPosition(X, Y);
            for (var i = 0; i < InputString.Length; i++) Console.Write(" ");
        }
    }
}