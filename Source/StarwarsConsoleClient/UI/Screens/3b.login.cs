using StarwarsConsoleClient.Main;
using System;
using System.IO;
using System.Threading.Tasks;
using static StarwarsConsoleClient.Main.Program;

namespace StarwarsConsoleClient.UI.Screens
{
    public static partial class Screen
    {
        public static async Task<Option> Login()
        {
            ConsoleWriter.ClearScreen();
            var lines = File.ReadAllLines(@"UI/maps/3b.Login.txt");
            var loginText = TextEditor.Add.DrawablesAt(lines, 0);
            TextEditor.Center.AllUnitsInXDir(loginText, Console.WindowWidth);
            TextEditor.Center.InYDir(loginText, Console.WindowHeight);
            ConsoleWriter.TryAppend(loginText);
            ConsoleWriter.Update();

            var colons = loginText.FindAll(x => x.Chars == ":");
            var nameLine = new InputLine(colons[0], 50, ForegroundColor);

            var accountName = nameLine.GetInputString(false);
            var passwordLine = new InputLine(colons[1], 50, ForegroundColor);
            var password = passwordLine.GetInputString(true);

            LineTools.ClearAt((nameLine.X, nameLine.Y), accountName);
            LineTools.ClearAt((passwordLine.X, passwordLine.Y), password);
            ConsoleWriter.ClearScreen();
            Console.SetCursorPosition(Console.WindowWidth/2-10, Console.WindowHeight/2 - 3);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Validating login...");

            bool success;
            success = await Client.LoginAsync(accountName, password);
           
            return success == true ? Option.Account : Option.Start;
        }
    }
}