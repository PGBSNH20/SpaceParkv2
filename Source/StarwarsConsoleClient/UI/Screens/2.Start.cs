using System;
using System.IO;

namespace StarwarsConsoleClient.UI.Screens
{
    public static partial class Screen
    {
        public static Option Start()
        {
            ConsoleWriter.ClearScreen();
            var lines = File.ReadAllLines(@"UI/maps/2.Start.txt");
            var drawables = TextEditor.Add.DrawablesAt(lines, 0);
            TextEditor.Center.ToScreen(drawables, Console.WindowWidth, Console.WindowHeight);
            var selectionList = new SelectionList<Option>(ConsoleColor.Green, '$');
            selectionList.GetCharPositions(drawables);
            selectionList.AddSelections(new[] { Option.Login, Option.Registration, Option.Exit });
            ConsoleWriter.TryAppend(drawables);
            ConsoleWriter.Update();

            return selectionList.GetSelection();
        }
    }
}