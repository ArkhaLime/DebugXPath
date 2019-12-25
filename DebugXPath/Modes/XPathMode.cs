using DebugXPath.Enums;
using DebugXPath.Helpers;
using System;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using CConsole = DebugXPath.Helpers.ColoredConsole;

namespace DebugXPath.Modes
{
    internal class XPathMode
    {
        private XmlDocument _document;
        XmlNamespaceManager _nsManager;
        private XmlNode _workNode = null;
        private XmlNode _selectedNode = null;

        private ESelectionModeStatus _selectionStatus = ESelectionModeStatus.None;
        private EExitMode _exitMode = EExitMode.None;

        private string command = string.Empty;
        private string separator;


        public XPathMode(XmlDocument xmlDocument)
        {
            _document = xmlDocument;
            _nsManager = NamespaceHelper.Instance.CreateNamespaceManagerFromDocument(_document);

            StringBuilder bld = new StringBuilder();
            bld.Append('-', 40);
            separator = bld.ToString();
        }

        public EExitMode Start()
        {
            string nsPrefix = _nsManager.LookupPrefix(_document.DocumentElement.NamespaceURI);
            if (string.IsNullOrWhiteSpace(nsPrefix))
            {
                DisplayDefaultNamespaceError(_document.DocumentElement.NamespaceURI);
                return EExitMode.ExitMode;
            }
            else
            {
                DisplayDefaultNamespace(_document.DocumentElement.NamespaceURI, nsPrefix);
            }

            Console.WriteLine("DocumentElement:");
            Console.WriteLine(_document.DocumentElement.OuterXml);
            Console.WriteLine();

            CConsole.WriteLine("Enter a XPath string (or 'exit').", ConsoleColor.Cyan);
            CConsole.WriteLine("Available commands :", ConsoleColor.Cyan);
            CConsole.WriteLine($" * {CommandHelper.SELECT_COMMAND} <xpath> : select a specific node to work with.", ConsoleColor.Cyan);
            CConsole.WriteLine($" * {CommandHelper.NODES_COMMAND} : list child nodes.", ConsoleColor.Cyan);

            XmlNodeList nodeList = null;

            while (true)
            {
                string prompt = "XPath > ";
                ConsoleColor color = ConsoleColor.Cyan;
                _workNode = _document.DocumentElement;

                if (_selectionStatus == ESelectionModeStatus.In)
                {
                    prompt = "XPath (selection) > ";
                    color = ConsoleColor.Magenta;
                    _workNode = _selectedNode;
                }

                CConsole.Write(prompt, color);
                command = Console.ReadLine();

                //Gestion de l'entrée utilisateur et des commandes
                if (string.IsNullOrWhiteSpace(command)) continue;
                if (CommandHelper.IsExitAllKeyword(command))
                {
                    _exitMode = EExitMode.ExitApplication;
                    break;
                }
                if (CommandHelper.IsExitKeyword(command))
                {
                    if (_selectionStatus == ESelectionModeStatus.In)
                    {
                        _selectionStatus = ESelectionModeStatus.None;
                        continue;
                    }
                    else
                    {
                        _exitMode = EExitMode.ExitMode;
                        break;
                    }
                }

                if (CommandHelper.IsSelectCommand(command))
                {
                    command = command.Replace(CommandHelper.SELECT_COMMAND, "").TrimStart();
                    if (string.IsNullOrWhiteSpace(command))
                    {
                        CConsole.WriteLine("Usage: /select <xpath>", ConsoleColor.Yellow);
                        continue;
                    }
                    _selectionStatus = ESelectionModeStatus.Entering;
                }

                if (CommandHelper.IsNodesCommand(command))
                {
                    DisplayChildNodeList(_workNode, color);
                    continue;
                }

                try
                {
                    nodeList = _workNode.SelectNodes(command, _nsManager);

                    if (_selectionStatus == ESelectionModeStatus.Entering && nodeList.Count != 1)
                    {
                        CConsole.WriteLine("Exiting selection mode. You must select only 1 node!", ConsoleColor.Yellow);
                        Console.WriteLine();
                        _selectionStatus = ESelectionModeStatus.None;
                    }

                    DisplayNodeList(nodeList, command, color);

                    if (_selectionStatus == ESelectionModeStatus.Entering && nodeList.Count > 0)
                    {
                        _selectedNode = nodeList[0];
                        _selectionStatus = ESelectionModeStatus.In;
                    }
                }
                catch (XPathException ex)
                {
                    CConsole.Write("Error with the xpath expression: '",ConsoleColor.Red);
                    CConsole.Write(command);
                    CConsole.WriteLine("'!", ConsoleColor.Red);
                    CConsole.WriteLine($"Message: {ex.Message}", ConsoleColor.Red);
                    Console.WriteLine();
                }

            } //end while

            return _exitMode;
        }

        private void DisplayNodeList(XmlNodeList nodeList, string xpath, ConsoleColor color)
        {
            if (nodeList.Count == 0)
            {
                //CConsole.WriteLine($"No XmlNode for XPath '{xpath}'",ConsoleColor.Yellow);
                CConsole.Write("No nodes for XPath '", ConsoleColor.Yellow);
                CConsole.Write(xpath, color);
                CConsole.WriteLine("'.", ConsoleColor.Yellow);
                Console.WriteLine();
                return;
            }

            DisplayNodes(nodeList, color);
        }

        private void DisplayChildNodeList(XmlNode workNode, ConsoleColor color)
        {
            var nodeList = workNode.ChildNodes;

            if (nodeList.Count == 0)
            {
                //CConsole.WriteLine($"No XmlNode for XPath '{xpath}'",ConsoleColor.Yellow);
                CConsole.Write("No child nodes for node '", ConsoleColor.Yellow);
                CConsole.Write(workNode.Name, color);
                CConsole.WriteLine("'!", ConsoleColor.Yellow);
                Console.WriteLine();
                return;
            }

            DisplayNodes(nodeList, color);
        }

        private void DisplayNodes(XmlNodeList nodeList, ConsoleColor color)
        {
            Console.WriteLine(separator);
            foreach (XmlNode node in nodeList)
            {
                //Console.WriteLine($"Node '{node.Name}'");
                Console.Write("Node '");
                CConsole.Write(node.Name, color);
                Console.WriteLine("' :");
                Console.WriteLine(node.OuterXml);
                Console.WriteLine();
                Console.WriteLine(separator);
            }
            CConsole.WriteLine($"Found {nodeList.Count} nodes.", color);
            Console.WriteLine();
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
            CConsole.WriteLine($"Add that namespace uri with a prefix in the '{NamespaceHelper.NAMESPACES_FILE_NAME}' file.", ConsoleColor.Red);
            Console.WriteLine();
        }
    }
}
