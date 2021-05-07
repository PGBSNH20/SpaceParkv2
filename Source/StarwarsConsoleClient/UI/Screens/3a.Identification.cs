﻿using StarwarsConsoleClient.Main;
using System;
using System.IO;
using System.Threading;
using static StarwarsConsoleClient.Main.Program;

namespace StarwarsConsoleClient.UI.Screens
{
    public static partial class Screen
    {
        public static Option Identification()
        {
            ConsoleWriter.ClearScreen();
            var lines = File.ReadAllLines(@"UI/maps/3a.Identification.txt");
            var drawables = TextEditor.Add.DrawablesAt(lines, 0);
            TextEditor.Center.ToScreen(drawables, Console.WindowWidth, Console.WindowHeight);
            ConsoleWriter.TryAppend(drawables);
            ConsoleWriter.Update();
            Console.CursorVisible = true;
            Console.ForegroundColor = ConsoleColor.Green;

            Func<string, (int X, int Y)> GetCoord = chr =>
            {
                var drawable = drawables.Find(x => x.Chars == chr);
                return (drawable.CoordinateX, drawable.CoordinateY);
            };
            Action<string> Clear = clear =>
            {
                Console.CursorVisible = false;
                foreach (var c in clear) Console.Write(" ");
                Console.CursorVisible = true;
            };

            //TODO input new inputs Identification with no question?
            var fCoord = GetCoord("F");
            var nameEndCoord = GetCoord(":");

            LineTools.SetCursor((nameEndCoord.X + 2, nameEndCoord.Y));
            var username = Console.ReadLine();

            LineTools.ClearAt(fCoord, "First name: " + username);
            Console.Write("Security question loading...");

            Func<string, string> getSecurityAnswer = securityQuestion =>
            {
                LineTools.ClearAt(fCoord, "Security question loading...");
                Console.Write(securityQuestion + " ");
                return Console.ReadLine();
            };

            var user = AccountManagement.IdentifyWithQuestion(username, getSecurityAnswer); //test

            LineTools.ClearAt(fCoord, "Security question loading... plus the long answer that i cleared now!");

            if (user == null)
            {
                ConsoleWriter.ClearScreen();
                LineTools.SetCursor(fCoord);
                Console.Write("Incorrect answer or incorrect name input");
                Console.ReadKey(true);
                return Option.Start;
            }

            var registrationExists = AccountManagement.Exists(username, true);
            var userExists = AccountManagement.Exists(user.Name, false);

            if (registrationExists == false && userExists == false)
            {
                _account.User = user;
                return Option.Registration;
            }


            LineTools.SetCursor(fCoord);
            Console.WriteLine("User is already registered");
            Thread.Sleep(1000);
            return Option.Start;
        }
    }
}