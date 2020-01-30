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
        public const string SHOW_COMMAND = "/show";
        public const string NAMESPACES_COMMAND = "/namespaces";
        public const string NAMESPACES_COMMAND_SHORT = "/ns";
        public const string NS_DISPLAY_COMMAND = "display";
        public const string NS_ADD_COMMAND = "add";
        public const string NS_SAVE_COMMAND = "save";
        public const string NS_DELETE_COMMAND = "delete";

        public const string TRIMMABLE = "\" ";

        public static bool IsExitAllKeyword(string command)
        {
            //return (command.Equals(EXIT_ALL_KEYWORD, StringComparison.InvariantCultureIgnoreCase));
            return Equals(command, EXIT_ALL_KEYWORD);
        }

        public static bool IsExitKeyword(string command)
        {
            //return (command.Equals(EXIT_KEYWORD, StringComparison.InvariantCultureIgnoreCase));
            return Equals(command, EXIT_KEYWORD);
        }

        public static bool IsHelpKeyword(string command)
        {
            //return (command.Equals(HELP_KEYWORD, StringComparison.InvariantCultureIgnoreCase));
            return Equals(command, HELP_KEYWORD);
        }

        public static bool IsNodesCommand(string command)
        {
            //return (command.Equals(NODES_COMMAND, StringComparison.InvariantCultureIgnoreCase));
            return Equals(command, NODES_COMMAND);
        }
        public static bool IsShowCommand(string command)
        {
            //return (command.Equals(NODES_COMMAND, StringComparison.InvariantCultureIgnoreCase));
            return Equals(command, SHOW_COMMAND);
        }

        public static bool IsSelectCommand(string command)
        {
            //return (command.StartsWith(SELECT_COMMAND, StringComparison.InvariantCultureIgnoreCase));
            return StartWith(command, SELECT_COMMAND);
        }

        public static bool IsNamespacesCommand(string command)
        {
            /*return (command.Equals(NAMESPACES_COMMAND, StringComparison.InvariantCultureIgnoreCase) ||
                command.Equals(NAMESPACES_COMMAND_SHORT, StringComparison.InvariantCultureIgnoreCase));*/

            return Equals(command, NAMESPACES_COMMAND) || Equals(command, NAMESPACES_COMMAND_SHORT);
        }

        public static bool IsNsDisplayCommand(string command)
        {
            //return (command.Equals(NS_DISPLAY_COMMAND, StringComparison.InvariantCultureIgnoreCase));
            return Equals(command, NS_DISPLAY_COMMAND);
        }

        public static bool IsNsAddCommand(string command)
        {
            //return (command.StartsWith(NS_ADD_COMMAND, StringComparison.InvariantCultureIgnoreCase));
            return StartWith(command, NS_ADD_COMMAND);
        }

        public static bool IsNsSaveCommand(string command)
        {
            //return (command.Equals(NS_SAVE_COMMAND, StringComparison.InvariantCultureIgnoreCase));
            return Equals(command, NS_SAVE_COMMAND);
        }

        public static bool IsNsDeleteCommand(string command)
        {
            //return (command.StartsWith(NS_DELETE_COMMAND, StringComparison.InvariantCultureIgnoreCase));
            return StartWith(command, NS_DELETE_COMMAND);
        }

        private static bool Equals(string userCommand, string commandConst)
        {
            userCommand = userCommand.Trim(TRIMMABLE.ToCharArray());
            commandConst = commandConst.Trim(TRIMMABLE.ToCharArray());

            return userCommand.Equals(commandConst, StringComparison.InvariantCultureIgnoreCase);
        }

        private static bool StartWith(string userCommand, string commandConst)
        {
            userCommand = userCommand.Trim(TRIMMABLE.ToCharArray());
            commandConst = commandConst.Trim(TRIMMABLE.ToCharArray());

            return userCommand.StartsWith(commandConst, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
