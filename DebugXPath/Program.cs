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
        private const string NAMESPACES_FILE_NAME = "Namespaces.txt";

        private static Dictionary<string, string> _namespaces;

        private static bool _startWithParameter = false;

        private static readonly Encoding utf8 = new UTF8Encoding(false);

        private static string separator;

        static void Main(string[] args)
        {
            try
            {
                string path = string.Empty;
                string xpath = string.Empty;

                Console.OutputEncoding = utf8;
                Console.InputEncoding = utf8;

                StringBuilder bld = new StringBuilder();
                bld.Append('-', 40);
                separator = bld.ToString();

                LoadNamespaces();

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

                    XmlDocument doc = new XmlDocument();
                    doc.Load(path);

                    XmlNamespaceManager nsManager = new XmlNamespaceManager(doc.NameTable);
                    AddNamespacesToManager(nsManager);
                    Console.WriteLine();

                    string nsPrefix = nsManager.LookupPrefix(doc.DocumentElement.NamespaceURI);
                    if (string.IsNullOrWhiteSpace(nsPrefix))
                    {
                        DisplayDefaultNamespaceError(doc.DocumentElement.NamespaceURI);
                        continue;
                    }
                    else
                    {
                        DisplayDefaultNamespace(doc.DocumentElement.NamespaceURI, nsPrefix);
                    }

                    Console.WriteLine("DocumentElement:");
                    Console.WriteLine(doc.DocumentElement.OuterXml);
                    Console.WriteLine();

                    XPathMode mode = new XPathMode(doc, nsManager);
                    EExitMode exitMode = mode.Start();

                    if (exitMode == EExitMode.ExitApplication) break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine();
                Console.WriteLine("Press a key to exit ...");
                Console.ReadKey();
            }

        }

        private static void LoadNamespaces()
        {
            _namespaces = new Dictionary<string, string>();

            Console.WriteLine("Loading custom namespaces...");

            string filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), NAMESPACES_FILE_NAME);
            if (File.Exists(filePath))
            {
                IEnumerable<string> lines = File.ReadLines(filePath);
                foreach (var line in lines)
                {
                    if (line.StartsWith("#")) continue; //on ne lit pas les commentaires

                    string[] parts = line.Split(";".ToCharArray(), 2, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2)
                    {
                        //Console.WriteLine($"\tAdding namespace '{parts[1]}' with prefix '{parts[0]}'");
                        Console.Write("\tAdding namespace '");
                        CConsole.Write(parts[1], ConsoleColor.Cyan);
                        Console.Write("' with prefix '");
                        CConsole.Write(parts[0], ConsoleColor.Cyan);
                        Console.WriteLine("'");
                        _namespaces.Add(parts[0], parts[1]);
                    }
                }
            }
            else
            {
                CConsole.WriteLine($"No '{NAMESPACES_FILE_NAME}' file. Creating it.", ConsoleColor.Yellow);
                try
                {
                    using (StreamWriter writer = File.CreateText(filePath))
                    {
                        writer.WriteLine("#Namespaces in format 'prefix;url'. Use semi-colon as separator. One namespace per line.");
                        writer.Flush();
                    }
                }
                catch (Exception ex)
                {
                    CConsole.WriteLine("Error when creating namespaces file. Msg: " + ex.Message, ConsoleColor.Red);
                }
            }

            Console.WriteLine($"{_namespaces.Count} namespaces loaded.");
            Console.WriteLine();
        }

        private static void AddNamespacesToManager(XmlNamespaceManager nsMan)
        {
            foreach (var key in _namespaces.Keys)
            {
                string value = string.Empty;
                _namespaces.TryGetValue(key, out value);
                if (key != string.Empty)
                    nsMan.AddNamespace(key, value);
            }
        }

        private static void DisplayDefaultNamespace(string uri, string prefix)
        {
            //Console.WriteLine($"Default namespace uri '{doc.DocumentElement.NamespaceURI}' has prefix '{nsPrefix}'.");
            Console.Write("Default namespace uri '");
            CConsole.Write(uri, ConsoleColor.Cyan);
            Console.Write("' has prefix '");
            CConsole.Write(prefix, ConsoleColor.Cyan);
            Console.WriteLine("'.");
            Console.WriteLine();
        }

        private static void DisplayDefaultNamespaceError(string uri)
        {
            //CConsole.WriteLine($"Can't find a prefix for default namespace uri '{doc.DocumentElement.NamespaceURI}' in namespace manager.", ConsoleColor.Red);
            //CConsole.WriteLine($"Add that namespace uri with a prefix in the '{NAMESPACES_FILE_NAME}' file.", ConsoleColor.Red);
            CConsole.Write("Can't find a prefix for default namespace uri '", ConsoleColor.Red);
            CConsole.Write(uri, ConsoleColor.Cyan);
            CConsole.WriteLine("' in namespace manager.", ConsoleColor.Red);
            CConsole.WriteLine($"Add that namespace uri with a prefix in the '{NAMESPACES_FILE_NAME}' file.", ConsoleColor.Red);
            Console.WriteLine();
        }
    }
}
