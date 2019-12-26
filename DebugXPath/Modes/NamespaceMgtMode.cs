using DebugXPath.Enums;
using DebugXPath.Helpers;
using System;
using System.Collections.Generic;
using CConsole = DebugXPath.Helpers.ColoredConsole;

namespace DebugXPath.Modes
{
    internal class NamespaceMgtMode
    {

        private const ConsoleColor MODE_COLOR = ConsoleColor.White;

        private EExitMode _exitMode = EExitMode.None;

        private string command = string.Empty;

        public NamespaceMgtMode() { }

        public EExitMode Start()
        {
            Console.WriteLine();
            DisplayHelp();

            while (true)
            {
                string prompt = "Namespaces > ";

                CConsole.Write(prompt, MODE_COLOR);
                command = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(command)) continue;

                if (CommandHelper.IsExitAllKeyword(command))
                {
                    _exitMode = EExitMode.ExitApplication;
                    break;
                }

                if (CommandHelper.IsExitKeyword(command))
                {
                    _exitMode = EExitMode.ExitMode;
                    break;
                }

                if (CommandHelper.IsHelpKeyword(command))
                {
                    DisplayHelp();
                    continue;
                }

                if (CommandHelper.IsNsDisplayCommand(command))
                {
                    DisplayCustomNamespaces();
                    continue;
                }

                if (CommandHelper.IsNsAddCommand(command))
                {
                    AddNamespace(command);
                    continue;
                }

            }

            return _exitMode;
        }

        private void DisplayHelp()
        {
            CConsole.WriteLine("Enter a command.", MODE_COLOR);
            CConsole.WriteLine("Available commands :", MODE_COLOR);
            CConsole.WriteLine($" * {CommandHelper.HELP_KEYWORD} : display this message", MODE_COLOR);
            CConsole.WriteLine($" * {CommandHelper.EXIT_KEYWORD} : exit the current mode", MODE_COLOR);
            CConsole.WriteLine($" * {CommandHelper.NS_DISPLAY_COMMAND} : display loaded custom namespaces", MODE_COLOR);
            CConsole.WriteLine($" * {CommandHelper.NS_ADD_COMMAND} <prefix> <uri> : add a custom namespace", MODE_COLOR);
            Console.WriteLine();
        }

        private void DisplayCustomNamespaces()
        {
            string status = string.Empty;

            CConsole.WriteLine("Loaded custom namespaces :", MODE_COLOR);
            foreach (KeyValuePair<string, string> kv in NamespaceHelper.Instance.GetCustomNamespaces())
            {
                status = string.IsNullOrWhiteSpace(kv.Key) ? " (unused)" : string.Empty;
                CConsole.Write(" * Namespace '", MODE_COLOR);
                CConsole.Write(kv.Value);
                CConsole.Write("' with prefix '", MODE_COLOR);
                CConsole.Write(kv.Key);
                CConsole.WriteLine("'" + status, MODE_COLOR);
            }
            Console.WriteLine();
        }

        private void AddNamespace(string command)
        {
            string[] parts = command.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 3)
            {
                CConsole.WriteLine($"Usage: {CommandHelper.NS_ADD_COMMAND} <prefix> <uri>", ConsoleColor.Yellow);
            }
            else
            {
                NamespaceHelper.Instance.AddNamespace(parts[1], parts[2]);
            }
            Console.WriteLine();
        }
    }
}
