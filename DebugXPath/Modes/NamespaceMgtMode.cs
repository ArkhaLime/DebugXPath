using DebugXPath.Enums;
using DebugXPath.Helpers;
using System;
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

            }

            return _exitMode;
        }

        private void DisplayHelp()
        {
            CConsole.WriteLine("Enter a command.", MODE_COLOR);
            CConsole.WriteLine("Available commands :", MODE_COLOR);
            CConsole.WriteLine($" * {CommandHelper.HELP_KEYWORD} : display this message", MODE_COLOR);
            CConsole.WriteLine($" * {CommandHelper.EXIT_KEYWORD} : exit the current mode", MODE_COLOR);
            Console.WriteLine();
        }

    }
}
