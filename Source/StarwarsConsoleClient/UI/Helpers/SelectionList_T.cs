using System;
using System.Collections.Generic;
using System.Linq;

namespace StarwarsConsoleClient.UI
{
    public class SelectionList<T>
    {
        public SelectionList(ConsoleColor foregroundColor, char selectChar)
        {
            ForegroundColor = foregroundColor;
            SelectChar = selectChar;
        }

        private T[] Selections { get; set; }
        private char SelectChar { get; }
        private ConsoleColor ForegroundColor { get; }
        private List<(int X, int Y)> XYs { get; set; }

        public void AddSelections(T[] selections)
        {
            Selections = selections;
        }

        public void GetCharPositions(List<IDrawable> drawables)
        {
            var targetsXY = new List<(int X, int Y)>();
            var removeTargets = new List<IDrawable>();

            foreach (var item in drawables)
                if (item.Chars == SelectChar.ToString())
                    removeTargets.Add(item);

            targetsXY = removeTargets.Select((x, y) => (x.CoordinateX, x.CoordinateY)).ToList();
            foreach (var remove in removeTargets) remove.IsDrawn = true;
            XYs = targetsXY;
        }

        public T GetSelection()
        {
            if (XYs.Count != Selections.Length) throw new Exception("selector chars and choices does not match");

            Console.CursorVisible = false;
            Console.ForegroundColor = ForegroundColor;
            var selIndex = 0;

            Console.SetCursorPosition(XYs[selIndex].X, XYs[selIndex].Y);
            Console.Write(SelectChar);
            var isDrawn = true;
            var keyInfo = Console.ReadKey(true);

            while (keyInfo.Key != ConsoleKey.Enter)
            {
                if (keyInfo.Key == ConsoleKey.UpArrow && selIndex > 0)
                {
                    selIndex--;
                    isDrawn = false;
                }

                if (keyInfo.Key == ConsoleKey.DownArrow && selIndex < Selections.Length - 1)
                {
                    selIndex++;
                    isDrawn = false;
                }

                if (isDrawn == false)
                {
                    Console.CursorLeft--;
                    Console.Write(' ');
                    Console.SetCursorPosition(XYs[selIndex].X, XYs[selIndex].Y);
                    Console.Write(SelectChar);
                    isDrawn = true;
                }

                keyInfo = Console.ReadKey(true);
            }

            return Selections[selIndex];
        }
    }
}