using System;
using System.IO;

namespace StarwarsConsoleClient.UI.Screens
{
    public static partial class Screen
    {
        public static Option Welcome()
        {
            ConsoleWriter.ClearScreen();
            var lines = File.ReadAllLines(@"UI/maps/1.Welcome.txt");
            var welcomeText = TextEditor.Add.DrawablesAt(lines, 0);
            TextEditor.Center.ToScreen(welcomeText, Console.WindowWidth, Console.WindowHeight);
            ConsoleWriter.TryAppend(welcomeText);
            ConsoleWriter.Update();

            return Option.Start;
        }
    }
}