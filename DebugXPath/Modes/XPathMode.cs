using DebugXPath.Enums;
using DebugXPath.Helpers;
using DebugXPath.Properties;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using CConsole = DebugXPath.Helpers.ColoredConsole;

namespace DebugXPath.Modes
{
    internal class XPathMode
    {
        private const ConsoleColor XPATH_MODE_COLOR = ConsoleColor.Cyan;
        private const ConsoleColor SELECTION_MODE_COLOR = ConsoleColor.Magenta;
        private const string XML_NAMESPACE = "xmlns:";

        private ConsoleColor? _activeColor;

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

            _activeColor = XPATH_MODE_COLOR;
        }

        public EExitMode Start()
        {
            Console.WriteLine("DocumentElement:");
            Console.WriteLine(_document.DocumentElement.OuterXml);
            Console.WriteLine();

            AddDocumentNamespaces();
            Console.WriteLine();

            string nsPrefix = _nsManager.LookupPrefix(_document.DocumentElement.NamespaceURI);

            if (string.IsNullOrWhiteSpace(nsPrefix))
            {
                DisplayDefaultNamespaceError(_document.DocumentElement.NamespaceURI);
                //return EExitMode.ExitMode;
            }
            else
            {
                DisplayDefaultNamespace(_document.DocumentElement.NamespaceURI, nsPrefix);
            }

            DisplayHelp();

            XmlNodeList nodeList = null;

            while (true)
            {
                string prompt = "XPath";
                _activeColor = XPATH_MODE_COLOR;
                _workNode = _document.DocumentElement;

                if (_selectionStatus == ESelectionModeStatus.In)
                {
                    prompt = "XPath (selection)";
                    _activeColor = SELECTION_MODE_COLOR;
                    _workNode = _selectedNode;
                }

                if (Settings.Default.DisplayCurrentNode)
                {
                    CConsole.Write($"{prompt} [", _activeColor);
                    CConsole.Write(FormatNodeName(_workNode));
                    CConsole.Write("] > ", _activeColor);
                }
                else
                {
                    CConsole.Write($"{prompt} > ", _activeColor);
                }
                
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

                    _exitMode = EExitMode.ExitMode;
                    break;
                }

                if (CommandHelper.IsHelpKeyword(command))
                {
                    DisplayHelp();
                    continue;
                }

                if (CommandHelper.IsSelectCommand(command))
                {
                    command = command.Replace(CommandHelper.SELECT_COMMAND, "").TrimStart();
                    if (string.IsNullOrWhiteSpace(command))
                    {
                        CConsole.WriteLine($"Usage: {CommandHelper.SELECT_COMMAND} <xpath>", ConsoleColor.Yellow);
                        continue;
                    }
                    if(command == "..")
                    {
                        if(_workNode != _document.DocumentElement)
                        {
                            _selectedNode = _selectedNode.ParentNode;
                            DisplayNode(_selectedNode);
                        }
                        else
                        {
                            CConsole.WriteLine("Can not select parent of the document node", ConsoleColor.Yellow);
                        }
                        continue;
                    }
                    _selectionStatus = ESelectionModeStatus.Entering;
                }

                if (CommandHelper.IsNodesCommand(command))
                {
                    DisplayChildNodeList(_workNode);
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

                    DisplayNodeList(nodeList, command);

                    if (_selectionStatus == ESelectionModeStatus.Entering && nodeList.Count > 0)
                    {
                        _selectedNode = nodeList[0];
                        _selectionStatus = ESelectionModeStatus.In;
                    }
                }
                catch (XPathException ex)
                {
                    CConsole.Write("Error with the xpath expression: '", ConsoleColor.Red);
                    CConsole.Write(command);
                    CConsole.WriteLine("'!", ConsoleColor.Red);
                    CConsole.WriteLine($"Message: {ex.Message}", ConsoleColor.Red);
                    Console.WriteLine();
                }

            } //end while

            return _exitMode;
        }

        private void DisplayHelp()
        {
            CConsole.WriteLine("Enter a XPath string.", _activeColor);
            CConsole.WriteLine("Available commands :", _activeColor);
            CConsole.WriteLine($" * {CommandHelper.HELP_KEYWORD} : display this message", _activeColor);
            CConsole.WriteLine($" * {CommandHelper.EXIT_KEYWORD} : exit the current mode", _activeColor);
            CConsole.WriteLine($" * {CommandHelper.NODES_COMMAND} : list child nodes.", _activeColor);
            CConsole.WriteLine($" * {CommandHelper.SELECT_COMMAND} <xpath> : select a specific node to work with.", _activeColor);
            Console.WriteLine();
        }

        private void DisplayNodeList(XmlNodeList nodeList, string xpath)
        {
            if (nodeList.Count == 0)
            {
                CConsole.Write("No nodes for XPath '", ConsoleColor.Yellow);
                CConsole.Write(xpath, _activeColor);
                CConsole.WriteLine("'.", ConsoleColor.Yellow);
                Console.WriteLine();
                return;
            }

            DisplayNodes(nodeList);
        }

        private void DisplayChildNodeList(XmlNode workNode)
        {
            var nodeList = workNode.ChildNodes;

            if (nodeList.Count == 0)
            {
                CConsole.Write("No child nodes for node '", ConsoleColor.Yellow);
                CConsole.Write(FormatNodeName(workNode), _activeColor);
                CConsole.WriteLine("'!", ConsoleColor.Yellow);
                Console.WriteLine();
                return;
            }

            DisplayNodes(nodeList);
        }

        private void DisplayNodes(XmlNodeList nodeList)
        {
            Console.WriteLine(separator);
            foreach (XmlNode node in nodeList)
            {
                DisplayNode(node);
            }
            CConsole.WriteLine($"Found {nodeList.Count} nodes.", _activeColor);
            Console.WriteLine();
        }

        private void DisplayNode(XmlNode node)
        {
            Console.Write("Node '");
            CConsole.Write(FormatNodeName(node), _activeColor);
            Console.WriteLine("' :");
            Console.WriteLine(node.OuterXml);
            Console.WriteLine();
            Console.WriteLine(separator);
        }

        private string FormatNodeName(XmlNode node)
        {
            string prefix = _nsManager.LookupPrefix(node.NamespaceURI);
            if (!string.IsNullOrWhiteSpace(prefix)) prefix += ":";
            return prefix + node.LocalName;
        }

        private void DisplayDefaultNamespace(string uri, string prefix)
        {
            Console.Write("Default namespace uri '");
            CConsole.Write(uri, _activeColor);
            Console.Write("' has prefix '");
            CConsole.Write(prefix, _activeColor);
            Console.WriteLine("'.");
            Console.WriteLine();
        }

        private void DisplayDefaultNamespaceError(string uri)
        {
            CConsole.Write("Can't find a prefix for default namespace uri '", ConsoleColor.Red);
            CConsole.Write(string.IsNullOrWhiteSpace(uri) ? "(empty)" : uri, _activeColor);
            CConsole.WriteLine("' in namespace manager.", ConsoleColor.Red);
            CConsole.WriteLine($"Add that namespace uri with a prefix in the '{NamespaceHelper.NAMESPACES_FILE_NAME}' file.", ConsoleColor.Red);
            CConsole.WriteLine("Maybe this file doesn't need the use of namespaces.", ConsoleColor.Yellow);
            Console.WriteLine();
        }

        private void AddDocumentNamespaces()
        {
            var attributes = _document.DocumentElement.Attributes;

            foreach (XmlAttribute attr in attributes)
            {
                if (!attr.Name.StartsWith(XML_NAMESPACE)) continue;

                string prefix = attr.Name.Replace(XML_NAMESPACE, "");
                if (string.IsNullOrWhiteSpace(prefix)) continue;

                _nsManager.AddNamespace(prefix, attr.Value);
                CConsole.Write("Adding namespace '");
                CConsole.Write(attr.Value, _activeColor);
                CConsole.Write("' with prefix '");
                CConsole.Write(prefix, _activeColor);
                CConsole.WriteLine("' from document.");
            }
        }
    }
}
