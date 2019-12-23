using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using CConsole = DebugXPath.ColoredConsole;

namespace DebugXPath
{
    class Program
    {
        private const string NAMESPACES_FILE_NAME = "Namespaces.txt";

        private const string EXIT_KEYWORD = "exit";
        private const string EXIT_ALL_KEYWORD = "qqq";
        private const string SELECT_COMMAND = "/select";
        private const string NODES_COMMAND = "/nodes";

        private static Dictionary<string, string> _namespaces;

        private static bool _inSelectionMode = false;
        private static bool _enterSelectionMode = false;
        private static XmlNode _selectedNode = null;
        private static XmlNode _workNode = null;

        private static readonly Encoding utf8 = new UTF8Encoding(false);

        private static StringBuilder separator;

        static void Main(string[] args)
        {
            try
            {
                string path = string.Empty;
                string xpath = string.Empty;

                Console.OutputEncoding = utf8;
                Console.InputEncoding = utf8;

                separator = new StringBuilder();
                separator.Append('-', 40);

                LoadNamespaces();

                CConsole.WriteLine("Enter a path to a xml file (or 'exit' to quit).",ConsoleColor.Green);
                while (true)
                {
                    CConsole.Write("File path > ",ConsoleColor.Green);
                    path = Console.ReadLine();

                    if (path == string.Empty) continue;
                    if (path.Equals(EXIT_KEYWORD, StringComparison.InvariantCultureIgnoreCase)) break;

                    if (!File.Exists(path))
                    {
                        CConsole.WriteLine($"Path '{path}' doesn't exists!",ConsoleColor.Red);
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

                    CConsole.WriteLine("Enter a XPath string (or 'exit').",ConsoleColor.Cyan);
                    CConsole.WriteLine("Available commands :",ConsoleColor.Cyan);
                    CConsole.WriteLine($" * {SELECT_COMMAND} <xpath> : select a specific node to work with.",ConsoleColor.Cyan);
                    CConsole.WriteLine($" * {NODES_COMMAND} : list child nodes.", ConsoleColor.Cyan);

                    XmlNodeList nodeList = null;

                    while (true)
                    {
                        string prompt = "XPath > ";
                        ConsoleColor color = ConsoleColor.Cyan;
                        _workNode = doc.DocumentElement;

                        if (_inSelectionMode)
                        {
                            prompt = "XPath (selection) > ";
                            color = ConsoleColor.Magenta;
                            _workNode = _selectedNode;
                        }

                        CConsole.Write(prompt,color);
                        xpath = Console.ReadLine();

                        //Gestion de l'entrée utilisateur et des commandes
                        if (xpath == string.Empty) continue;
                        if (xpath.Equals(EXIT_ALL_KEYWORD, StringComparison.InvariantCultureIgnoreCase)) break;
                        if (xpath.Equals(EXIT_KEYWORD, StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (_inSelectionMode)
                            {
                                _inSelectionMode = false;
                                continue;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (xpath.StartsWith(SELECT_COMMAND, StringComparison.InvariantCultureIgnoreCase))
                        {
                            _enterSelectionMode = true;
                            xpath = xpath.Replace(SELECT_COMMAND, "").Trim();
                        }

                        if (xpath.StartsWith(NODES_COMMAND, StringComparison.InvariantCultureIgnoreCase))
                        {
                            DisplayChildNodeList(_workNode, color);
                            continue;
                        }

                        nodeList = _workNode.SelectNodes(xpath, nsManager);

                        if (_enterSelectionMode && nodeList.Count != 1)
                        {
                            CConsole.WriteLine("Exiting selection mode. You must select only 1 node!",ConsoleColor.Yellow);
                            Console.WriteLine();
                            _enterSelectionMode = false;
                        }

                        DisplayNodeList(nodeList, xpath, color);

                        if (_enterSelectionMode && nodeList.Count > 0)
                        {
                            _selectedNode = nodeList[0];
                            _inSelectionMode = true;
                            _enterSelectionMode = false;
                        }
                    }

                    if (xpath.Equals(EXIT_ALL_KEYWORD, StringComparison.InvariantCultureIgnoreCase)) break;
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

            Console.WriteLine($"Loading custom namespaces...");

            string filePath = Path.Combine(Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location), NAMESPACES_FILE_NAME);
            if (File.Exists(filePath))
            {
                IEnumerable<string> lines = File.ReadLines(filePath);
                foreach (var line in lines)
                {
                    if (line.StartsWith("#")) continue; //on ne lit pas les commentaires

                    string[] parts = line.Split(";".ToCharArray(),2,StringSplitOptions.RemoveEmptyEntries);
                    if(parts.Length == 2)
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

                Console.WriteLine($"{_namespaces.Count} namespaces loaded.");
            }
            else
            {
                CConsole.WriteLine($"No '{NAMESPACES_FILE_NAME}' file. Creating it.",ConsoleColor.Yellow);
                try
                {
                    var writer = File.CreateText(filePath);
                    writer.WriteLine("#Namespace in format 'prefix;url'. Use semi-colon as separator. One namespace per line.");
                    writer.Close();
                }
                catch (Exception ex)
                {
                    CConsole.WriteLine("Error when creating namespaces file. Msg: " + ex.Message,ConsoleColor.Red);
                }
            }
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

        private static void DisplayNodeList(XmlNodeList nodeList, string xpath, ConsoleColor color)
        {
            if (nodeList.Count == 0)
            {
                //CConsole.WriteLine($"No XmlNode for XPath '{xpath}'",ConsoleColor.Yellow);
                CConsole.Write("No nodes for XPath '", ConsoleColor.Yellow);
                CConsole.Write(xpath,color);
                CConsole.WriteLine("'.", ConsoleColor.Yellow);
                Console.WriteLine();
                return;
            }

            DisplayNodes(nodeList, color);
        }

        private static void DisplayChildNodeList(XmlNode workNode, ConsoleColor color)
        {
            var nodeList = workNode.ChildNodes;

            if (nodeList.Count == 0)
            {
                //CConsole.WriteLine($"No XmlNode for XPath '{xpath}'",ConsoleColor.Yellow);
                CConsole.Write("No child nodes for node '", ConsoleColor.Yellow);
                CConsole.Write(workNode.Name,color);
                CConsole.WriteLine("'!", ConsoleColor.Yellow);
                Console.WriteLine();
                return;
            }

            DisplayNodes(nodeList, color);
        }

        private static void DisplayNodes(XmlNodeList nodeList, ConsoleColor color)
        {
            Console.WriteLine(separator.ToString());
            foreach (XmlNode node in nodeList)
            {
                //Console.WriteLine($"Node '{node.Name}'");
                Console.Write("Node '");
                CConsole.Write(node.Name, color);
                Console.WriteLine("' :");
                Console.WriteLine(node.OuterXml);
                Console.WriteLine();
                Console.WriteLine(separator.ToString());
            }
            CConsole.WriteLine($"Found {nodeList.Count} nodes.",color);
            Console.WriteLine();
        }
    }
}
