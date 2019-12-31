using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using CConsole = DebugXPath.Helpers.ColoredConsole;

namespace DebugXPath.Helpers
{
    class NamespaceHelper
    {

        public const string NAMESPACES_FILE_NAME = "Namespaces.txt";
        private const string FILE_HEADER = "# Namespaces in format 'prefix;uri'. Use semi-colon as separator. One namespace per line.";

        private static NamespaceHelper _instance = null;
        private static Dictionary<string, string> _namespaces;

        public static NamespaceHelper Instance
        {
            get
            {
                if (_instance == null) _instance = new NamespaceHelper();
                return _instance;
            }
        }

        private NamespaceHelper()
        {
            _namespaces = new Dictionary<string, string>();
            //LoadNamespaces();
        }

        private string GetFilePath()
        {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), NAMESPACES_FILE_NAME);
        }

        public void LoadNamespaces()
        {
            _namespaces.Clear();

            CConsole.WriteLine("Loading custom namespaces...");

            string filePath = GetFilePath();
            if (File.Exists(filePath))
            {
                IEnumerable<string> lines = File.ReadLines(filePath);
                foreach (var line in lines)
                {
                    if (line.StartsWith("#")) continue; //don't load comments

                    string[] parts = line.Split(";".ToCharArray(), 2, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2)
                    {
                        AddNamespace(parts[0], parts[1]);
                    }
                }
            }
            else
            {
                CreateNamespacesFile();
            }

            CConsole.WriteLine($"{_namespaces.Count} namespaces loaded.");
            CConsole.WriteLine();
        }

        private void CreateNamespacesFile()
        {
            CConsole.WriteLine($"'{NAMESPACES_FILE_NAME}' file doesn't exists. Creating it.", ConsoleColor.Yellow);
            try
            {
                using (StreamWriter writer = File.CreateText(GetFilePath()))
                {
                    writer.WriteLine(FILE_HEADER);
                    writer.Flush();
                }
            }
            catch (Exception ex)
            {
                CConsole.WriteLine("Error when creating namespaces file. Msg: " + ex.Message, ConsoleColor.Red);
            }
            CConsole.WriteLine();
        }

        public bool AddNamespace(string prefix, string uri)
        {
            if (_namespaces.ContainsKey(prefix))
            {
                CConsole.Write("Prefix '", ConsoleColor.Yellow);
                CConsole.Write(prefix);
                CConsole.Write("' is already in use!", ConsoleColor.Yellow);
                CConsole.WriteLine();
                return false;
            }

            CConsole.Write("Adding namespace '");
            CConsole.Write(uri, ConsoleColor.Cyan);
            CConsole.Write("' with prefix '");
            CConsole.Write(prefix, ConsoleColor.Cyan);
            CConsole.WriteLine("'.");
            CConsole.WriteLine();
            _namespaces.Add(prefix, uri);
            return true;
        }

        public bool DeleteNamespace(string prefix)
        {
            return _namespaces.Remove(prefix);
        }

        public XmlNamespaceManager CreateNamespaceManagerFromDocument(XmlDocument document)
        {
            XmlNamespaceManager nsManager = new XmlNamespaceManager(document.NameTable);

            /*foreach (string key in _namespaces.Keys)
            {
                if (string.IsNullOrWhiteSpace(key)) continue;
                string value = string.Empty;
                bool ok = _namespaces.TryGetValue(key, out value);
                if (ok) nsManager.AddNamespace(key, value);
            }*/

            foreach (KeyValuePair<string, string> kv in _namespaces)
            {
                if (string.IsNullOrWhiteSpace(kv.Key) || string.IsNullOrWhiteSpace(kv.Value)) continue;
                nsManager.AddNamespace(kv.Key, kv.Value);
            }

            return nsManager;
        }

        public IReadOnlyDictionary<string, string> GetCustomNamespaces()
        {
            return _namespaces;
        }

        public bool SaveNewNamespaces()
        {
            string path = GetFilePath();
            bool exists = File.Exists(path);

            try
            {
                if (exists) File.Delete(path);

                using (StreamWriter writer = File.CreateText(path))
                {
                    writer.WriteLine(FILE_HEADER);
                    foreach (KeyValuePair<string, string> kv in _namespaces)
                    {
                        writer.WriteLine($"{kv.Key};{kv.Value}");
                    }
                    writer.Flush();
                }

                return true;
            }
            catch (Exception ex)
            {
                CConsole.WriteLine("Error when saving namespaces file. Msg: " + ex.Message, ConsoleColor.Red);
                CConsole.WriteLine();
                return false;
            }

        }

    }
}
