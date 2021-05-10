using System;
using System.IO;
using System.Linq;
using static StarwarsConsoleClient.Main.Program;
using StarwarsConsoleClient.Main;
using System.Threading.Tasks;

namespace StarwarsConsoleClient.UI.Screens
{
    public static partial class Screen
    {
        public static async Task<Option> SelectSpacePort()
        {
            ConsoleWriter.ClearScreen();
            var lines = File.ReadAllLines(@"UI/maps/7.SelectSpacePort.txt");
            var drawables = TextEditor.Add.DrawablesAt(lines, 0);
            var nextLine = drawables.Max(x => x.CoordinateY);
            var ports = await Client.GetSpacePortsAsync(UserData.BaseAPIUrl + "/api/Parking/GetSpacePorts");
            var portLines = ports.Select(x => "$ " + x.name).ToArray();
            drawables.AddRange(TextEditor.Add.DrawablesAt(portLines, nextLine + 3));
            TextEditor.Center.ToScreen(drawables, Console.WindowWidth, Console.WindowHeight);

            var selectionList = new SelectionList<SpacePort>(ForegroundColor, '$');
            selectionList.GetCharPositions(drawables);
            selectionList.AddSelections(ports.ToArray());
            ConsoleWriter.TryAppend(drawables);
            ConsoleWriter.Update();

            var port = selectionList.GetSelection();
            UserData.SelectedSpacePort = port;
            return Option.Account;
        }
    }
}