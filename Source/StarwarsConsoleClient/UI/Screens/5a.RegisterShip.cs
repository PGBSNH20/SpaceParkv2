﻿using System;
using System.IO;
using System.Linq;
using static StarwarsConsoleClient.Main.Program;
using StarwarsConsoleClient.Main;
using System.Threading.Tasks;

namespace StarwarsConsoleClient.UI.Screens
{
    public static partial class Screen
    {
        public static async Task<Option> RegisterShip(bool reRegister = false)
        {
            ConsoleWriter.ClearScreen();
            var lines = File.ReadAllLines(@"UI/maps/5a.RegisterShip.txt");
            var drawables = TextEditor.Add.DrawablesAt(lines, 0);
            var nextLine = drawables.Max(x => x.CoordinateY);
            var ships = await Client.GetShipsAsync();
            var shipLines = ships.Select(x => "$ " + x.Model).ToArray();
            drawables.AddRange(TextEditor.Add.DrawablesAt(shipLines, nextLine + 3));
            TextEditor.Center.ToScreen(drawables, Console.WindowWidth, Console.WindowHeight);

            var selectionList = new SelectionList<SpaceShip>(ForegroundColor, '$');
            selectionList.GetCharPositions(drawables);
            selectionList.AddSelections(ships.ToArray());
            ConsoleWriter.TryAppend(drawables);
            ConsoleWriter.Update();

            var ship = selectionList.GetSelection();
            UserData.SpaceShip = ship;
            if (reRegister)
            {
                var status = await Client.ChangeSpaceShipAsync(ship.Model);
                return Option.Account;
            }

            var success = await Client.RegisterAsync(ship.Model, UserData.PersonName, UserData.AccountName, UserData.Password);
            return Option.Login;
        }
    }
}