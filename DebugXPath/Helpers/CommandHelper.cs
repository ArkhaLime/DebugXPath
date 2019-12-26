using System;

namespace DebugXPath.Helpers
{
    public static class CommandHelper
    {
        public const string EXIT_KEYWORD = "exit";
        public const string EXIT_ALL_KEYWORD = "qqq";
        public const string HELP_KEYWORD = "help";
        public const string SELECT_COMMAND = "/select";
        public const string NODES_COMMAND = "/nodes";
        public const string NAMESPACES_COMMAND = "/namespaces";
        public const string NAMESPACES_COMMAND_SHORT = "/ns";
        public const string NS_DISPLAY_COMMAND = "display";
        public const string NS_ADD_COMMAND = "add";

        public static bool IsExitAllKeyword(string command)
        {
            return (command.Equals(EXIT_ALL_KEYWORD, StringComparison.InvariantCultureIgnoreCase));
        }

        public static bool IsExitKeyword(string command)
        {
            return (command.Equals(EXIT_KEYWORD, StringComparison.InvariantCultureIgnoreCase));
        }

        public static bool IsHelpKeyword(string command)
        {
            return (command.Equals(HELP_KEYWORD, StringComparison.InvariantCultureIgnoreCase));
        }

        public static bool IsNodesCommand(string command)
        {
            return (command.Equals(NODES_COMMAND, StringComparison.InvariantCultureIgnoreCase));
        }

        public static bool IsSelectCommand(string command)
        {
            return (command.StartsWith(SELECT_COMMAND, StringComparison.InvariantCultureIgnoreCase));
        }

        public static bool IsNamespacesCommand(string command)
        {
            return (command.Equals(NAMESPACES_COMMAND, StringComparison.InvariantCultureIgnoreCase) ||
                command.Equals(NAMESPACES_COMMAND_SHORT, StringComparison.InvariantCultureIgnoreCase));
        }

        public static bool IsNsDisplayCommand(string command)
        {
            return (command.Equals(NS_DISPLAY_COMMAND, StringComparison.InvariantCultureIgnoreCase));
        }

        public static bool IsNsAddCommand(string command)
        {
            return (command.StartsWith(NS_ADD_COMMAND, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
