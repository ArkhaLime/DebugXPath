using DebugXPath.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CConsole = DebugXPath.Helpers.ColoredConsole;
using DebugXPath.Helpers;

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


        public XPathMode(XmlDocument xmlDocument, XmlNamespaceManager nsManager)
        {
            _document = xmlDocument;
            _nsManager = nsManager;

            StringBuilder bld = new StringBuilder();
            bld.Append('-', 40);
            separator = bld.ToString();
        }

        public EExitMode Start()
        {
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
    }
}
