using System;

namespace StarwarsConsoleClient.UI
{
    public interface IDrawable
    {
        public int CoordinateX { get; set; }
        public int CoordinateY { get; set; }
        public string Chars { get; set; }
        public ConsoleColor BackgroundColor { get; set; }
        public ConsoleColor ForegroundColor { get; set; }
        public bool IsDrawn { get; set; }
        public bool Erase { get; set; }
        public bool IsSame(IDrawable drawable);
    }
}