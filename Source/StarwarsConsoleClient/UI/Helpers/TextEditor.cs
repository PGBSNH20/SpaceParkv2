using System;
using System.Collections.Generic;
using System.Linq;

namespace StarwarsConsoleClient.UI
{
    public static class TextEditor
    {
        private static ConsoleColor ForegroundColor { get; } = ConsoleColor.Green;

        public static class Center
        {
            public static void ToScreen(List<IDrawable> drawables, int screenWidth, int screenHeight)
            {
                AllUnitsInXDir(drawables, screenWidth);
                InYDir(drawables, screenHeight);
            }

            public static void InXDir(List<IDrawable> drawables, int screenWidth)
            {
                var xMax = drawables.Max(x => x.CoordinateX);
                var xMin = drawables.Min(x => x.CoordinateX);
                var xDiff = xMax - xMin;
                var leftColumnPosition = (screenWidth - xDiff) / 2;
                var move = leftColumnPosition - xMin;
                drawables.ForEach(x => x.CoordinateX += move);
            }

            public static void AllUnitsInXDir(List<IDrawable> drawables, int screenWidth)
            {
                var listOfLists = Units.GetRowUnits(drawables);
                foreach (var list in listOfLists) InXDir(list, screenWidth);
            }

            public static void InYDir(List<IDrawable> drawables, int screenHeight)
            {
                var yMax = drawables.Max(x => x.CoordinateY);
                var yMin = drawables.Min(x => x.CoordinateY);
                var yDiff = yMax - yMin;
                var topRowPosition = (screenHeight - yDiff) / 2;
                var move = topRowPosition - yMin;
                drawables.ForEach(x => x.CoordinateY += move);
            }
        }

        public static class Units
        {
            public static List<int> GetBlankRows(List<IDrawable> drawables)
            {
                var blanks = new List<int>();

                var yMin = drawables.Select(x => x.CoordinateY).Min();
                var yMAx = drawables.Select(x => x.CoordinateY).Max();

                for (var y = yMin; y <= yMAx; y++)
                {
                    var yExist = drawables.Exists(x => x.CoordinateY == y);
                    if (yExist == false)
                        blanks.Add(y);
                }

                return blanks;
            }

            public static List<IDrawable> GetRowUnitAt(List<IDrawable> drawables, int unitIndex)
            {
                return GetRowUnits(drawables)[unitIndex];
            }

            public static List<List<IDrawable>> GetRowUnits(List<IDrawable> drawables)
            {
                var unitDrawables = new List<List<IDrawable>>();
                var blankRows = GetBlankRows(drawables);
                var yMin = drawables.Select(x => x.CoordinateY).Min();
                var yMAx = drawables.Select(x => x.CoordinateY).Max();

                blankRows.Insert(0, yMin);
                blankRows.Add(yMAx);

                for (var i = 0; i < blankRows.Count - 1; i++)
                {
                    var unit = drawables.FindAll(
                        x => x.CoordinateY >= blankRows[i] && x.CoordinateY <= blankRows[i + 1]);
                    if (unit.Count != 0)
                        unitDrawables.Add(unit);
                }

                return unitDrawables;
            }
        }

        public static class Add
        {
            public static List<IDrawable> DrawablesAt(string[] textLines, int firstRow)
            {
                var drawables = new List<IDrawable>();

                var x = 0;
                var y = firstRow;

                foreach (var line in textLines)
                {
                    foreach (var chr in line)
                    {
                        if (chr != ' ')
                        {
                            var drawable = new TextDrawable
                            {
                                CoordinateX = x,
                                CoordinateY = y,
                                ForegroundColor = ForegroundColor,
                                Chars = chr.ToString()
                            };
                            drawables.Add(drawable);
                        }

                        x++;
                    }

                    y++;
                    x = 0;
                }

                return drawables;
            }

            public static void DrawablesWithSpacing(List<IDrawable> drawables, string[] textLines, int spacing)
            {
                var Ymax = drawables.Select(x => x.CoordinateY).Max();
                var newDrawables = DrawablesAt(textLines, Ymax + spacing);
                drawables.AddRange(newDrawables);
            }
        }
    }
}