using DebugXPath.Enums;
using DebugXPath.Helpers;
using DebugXPath.Modes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using CConsole = DebugXPath.Helpers.ColoredConsole;

namespace DebugXPath
{
    class Program
    {
        private static readonly Encoding utf8 = new UTF8Encoding(false);

        private static bool _startWithParameter = false;

        static void Main(string[] args)
        {
            try
            {
                string path = string.Empty;
                string xpath = string.Empty;

                Console.OutputEncoding = utf8;
                Console.InputEncoding = utf8;

                NamespaceHelper.Instance.LoadNamespaces();

                if (args.Length > 0)
                {
                    path = args[0];
                    _startWithParameter = true;
                }

                CConsole.WriteLine("Enter a path to a xml file (or 'exit' to quit).", ConsoleColor.Green);

                #region "file mode"
                while (true)
                {
                    CConsole.Write("File path > ", ConsoleColor.Green);

                    if (_startWithParameter)
                    {
                        Console.WriteLine(path);
                        _startWithParameter = false;
                    }
                    else
                    {
                        path = Console.ReadLine();
                    }

                    if (string.IsNullOrWhiteSpace(path)) continue;
                    if (CommandHelper.IsExitKeyword(path) || CommandHelper.IsExitAllKeyword(path)) break;

                    if (!File.Exists(path))
                    {
                        CConsole.WriteLine($"Path '{path}' doesn't exists!", ConsoleColor.Red);
                        continue;
                    }

                    Console.WriteLine();

                    XmlDocument doc = new XmlDocument();
                    doc.Load(path);

                    XPathMode mode = new XPathMode(doc);
                    EExitMode exitMode = mode.Start();

                    if (exitMode == EExitMode.ExitApplication) break;
                }
                #endregion "file mode"
            }
            catch (Exception ex)
            {
                CConsole.WriteLine(ex.ToString(),ConsoleColor.Red);
                Console.WriteLine();
                Console.WriteLine("Press a key to exit ...");
                Console.ReadKey();
            }

        }
    }
}
