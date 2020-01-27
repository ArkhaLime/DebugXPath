using DebugXPath.Enums;
using DebugXPath.Helpers;
using DebugXPath.Modes;
using System;
using System.IO;
using System.Text;
using System.Xml;
using CConsole = DebugXPath.Helpers.ColoredConsole;

namespace DebugXPath
{
    class Program
    {
        private const ConsoleColor MODE_COLOR = ConsoleColor.Green;
        private static readonly Encoding utf8 = new UTF8Encoding(false);

        private static bool _startWithParameter = false;

        static void Main(string[] args)
        {
            try
            {
                string path = string.Empty;
                string xpath = string.Empty;

                Console.OutputEncoding = utf8;
                //removed input encoding utf8 because character like "é" are treated like "\0" when entered in cmd and in windows terminal.
                //Console.InputEncoding = utf8;

                NamespaceHelper.Instance.LoadNamespaces();

                if (args.Length > 0)
                {
                    path = args[0];
                    _startWithParameter = true;
                }

                DisplayHelp();

                #region "file mode"
                while (true)
                {
                    CConsole.Write("File path > ", MODE_COLOR);

                    if (_startWithParameter)
                    {
                        CConsole.WriteLine(path);
                        _startWithParameter = false;
                    }
                    else
                    {
                        path = Console.ReadLine();
                    }

                    path = path.Trim(CommandHelper.TRIMMABLE.ToCharArray());

                    if (string.IsNullOrWhiteSpace(path)) continue;

                    if (CommandHelper.IsExitKeyword(path) || CommandHelper.IsExitAllKeyword(path)) break;

                    if (CommandHelper.IsHelpKeyword(path))
                    {
                        DisplayHelp();
                        continue;
                    }

                    if (CommandHelper.IsNamespacesCommand(path))
                    {
                        EExitMode nsExitMode = new NamespaceMgtMode().Start();
                        //if (nsExitMode == EExitMode.ExitMode) continue;
                        if (nsExitMode == EExitMode.ExitApplication) break;

                        //use continue for EExitMode.None (is usefull ?) and EExitMode.ExitMode
                        continue;
                    }

                    if (!File.Exists(path))
                    {
                        CConsole.WriteLine($"Path '{path}' doesn't exists!", ConsoleColor.Red);
                        CConsole.WriteLine();
                        continue;
                    }

                    try
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.Load(path);

                        Console.WriteLine();
                        XPathMode mode = new XPathMode(doc);
                        EExitMode exitMode = mode.Start();

                        if (exitMode == EExitMode.ExitApplication) break;
                    }
                    catch (XmlException ex)
                    {
                        CConsole.WriteLine("Error when loading file!", ConsoleColor.Red);
                        CConsole.WriteLine($"Message: {ex.Message}", ConsoleColor.Red);
                        Console.WriteLine();
                    }
                }
                #endregion "file mode"
            }
            catch (Exception ex)
            {
                CConsole.WriteLine(ex.ToString(),ConsoleColor.Red);
                CConsole.WriteLine();
                CConsole.WriteLine("Press a key to exit ...");
                Console.ReadKey();
            }

        }

        private static void DisplayHelp()
        {
            CConsole.WriteLine("Enter a path to a xml file.", MODE_COLOR);
            CConsole.WriteLine("Available commands :", MODE_COLOR);
            CConsole.WriteLine($" * {CommandHelper.HELP_KEYWORD} : display this message", MODE_COLOR);
            CConsole.WriteLine($" * {CommandHelper.EXIT_KEYWORD} : exit the application", MODE_COLOR);
            CConsole.WriteLine($" * {CommandHelper.NAMESPACES_COMMAND} or {CommandHelper.NAMESPACES_COMMAND_SHORT} : enter namespaces mode", MODE_COLOR);
            CConsole.WriteLine();
        }
    }
}
