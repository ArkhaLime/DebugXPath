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

        private static bool _hasModificationsNotSaved = false;

        private string command = string.Empty;

        public NamespaceMgtMode() { }

        public EExitMode Start()
        {
            Console.WriteLine();
            DisplayHelp();

            while (true)
            {
                string prompt = "Namespaces > ";
                if (_hasModificationsNotSaved) prompt = "Namespaces * > ";

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

                if (CommandHelper.IsNsDeleteCommand(command))
                {
                    DeleteNamespace(command);
                    continue;
                }

                if (CommandHelper.IsNsSaveCommand(command))
                {
                    SaveNewNamespaces();
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
            CConsole.WriteLine($" * {CommandHelper.NS_DELETE_COMMAND} <prefix> : delete the custom namespace associated with <prefix>", MODE_COLOR);
            CConsole.WriteLine($" * {CommandHelper.NS_SAVE_COMMAND} : save added namespaces", MODE_COLOR);
            Console.WriteLine();
        }

        private void DisplayCustomNamespaces()
        {
            string status = string.Empty;

            CConsole.WriteLine("Loaded custom namespaces :", MODE_COLOR);
            foreach (KeyValuePair<string, string> kv in NamespaceHelper.Instance.GetCustomNamespaces())
            {
                status = string.Empty;
                if (string.IsNullOrWhiteSpace(kv.Key) || string.IsNullOrWhiteSpace(kv.Value))
                {
                    status = " (unusable)";
                }
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
                bool added = NamespaceHelper.Instance.AddNamespace(parts[1], parts[2]);
                _hasModificationsNotSaved |= added;
            }
            Console.WriteLine();
        }

        private void DeleteNamespace(string command)
        {
            string[] parts = command.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
            {
                CConsole.WriteLine($"Usage: {CommandHelper.NS_DELETE_COMMAND} <prefix>", ConsoleColor.Yellow);
            }
            else
            {
                bool removed = NamespaceHelper.Instance.DeleteNamespace(parts[1]);
                _hasModificationsNotSaved |= removed;

                CConsole.Write("Prefix '", MODE_COLOR);
                CConsole.Write(parts[1]);
                CConsole.WriteLine("' was removed.", MODE_COLOR);
            }
            Console.WriteLine();
        }

        private void SaveNewNamespaces()
        {
            bool saved = NamespaceHelper.Instance.SaveNewNamespaces();

            if (saved)
            {
                CConsole.WriteLine("Namespaces saved!", MODE_COLOR);
                _hasModificationsNotSaved = false;
            }
            Console.WriteLine();
        }
    }
}
