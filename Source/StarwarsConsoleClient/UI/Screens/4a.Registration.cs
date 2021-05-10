using StarwarsConsoleClient.Main;
using System;
using System.IO;

namespace StarwarsConsoleClient.UI.Screens
{
    public static partial class Screen
    {
        public static Option Registration()
        {
            ConsoleWriter.ClearScreen();
            var lines = File.ReadAllLines(@"UI/maps/4a.Registration.txt");
            var drawables = TextEditor.Add.DrawablesAt(lines, 0);
            TextEditor.Center.AllUnitsInXDir(drawables, Console.WindowWidth);
            TextEditor.Center.InYDir(drawables, Console.WindowHeight);
            ConsoleWriter.TryAppend(drawables);
            ConsoleWriter.Update();

            Func<IDrawable, (int X, int Y)> getCoord = drawable => (drawable.CoordinateX + 2, drawable.CoordinateY);

            var colons = drawables.FindAll(x => x.Chars == ":");

            var fullnameCoord = getCoord(colons[0]);
            var nameCoord = getCoord(colons[1]);
            var pass1Coord = getCoord(colons[2]);
            var pass2Coord = getCoord(colons[3]);

            Console.ForegroundColor = ConsoleColor.Green;
            LineTools.SetCursor(nameCoord);

            var fullnameLine = new InputLine(fullnameCoord.X, fullnameCoord.Y, 30, ConsoleColor.Green);
            var nameLine = new InputLine(nameCoord.X, nameCoord.Y, 30, ConsoleColor.Green);
            var pass1Line = new InputLine(pass1Coord.X, pass1Coord.Y, 30, ConsoleColor.Green);
            var pass2Line = new InputLine(pass2Coord.X, pass2Coord.Y, 30, ConsoleColor.Green);

            var fullname = "";
            var accountName = "";
            var password1 = "";
            var password2 = "";

            do
            {
                fullname = fullnameLine.GetInputString(false);
                accountName = nameLine.GetInputString(false);
                password1 = pass1Line.GetInputString(true);
                password2 = pass2Line.GetInputString(true);

                LineTools.ClearAt(fullnameCoord, fullname);
                LineTools.ClearAt(nameCoord, accountName);
                LineTools.ClearAt(pass1Coord, password1);
                LineTools.ClearAt(pass2Coord, password2);
            } while (password1 != password2 || accountName.Length <= 5 || password1.Length <= 5);

            UserData.tryFullName = fullname;
            UserData.AccountName = accountName;
            UserData.Password = password1;

            return Option.RegisterShip;
        }
    }
}