using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebugXPath.Helpers
{
    public static class CommandHelper
    {
        public const string EXIT_KEYWORD = "exit";
        public const string EXIT_ALL_KEYWORD = "qqq";
        public const string SELECT_COMMAND = "/select";
        public const string NODES_COMMAND = "/nodes";

        public static bool IsExitAllKeyword(string command)
        {
            return (command.Equals(EXIT_ALL_KEYWORD, StringComparison.InvariantCultureIgnoreCase));
        }

        public static bool IsExitKeyword(string command)
        {
            return (command.Equals(EXIT_KEYWORD, StringComparison.InvariantCultureIgnoreCase));
        }

        public static bool IsNodesCommand(string command)
        {
            return (command.Equals(NODES_COMMAND, StringComparison.InvariantCultureIgnoreCase));
        }

        public static bool IsSelectCommand(string command)
        {
            return (command.StartsWith(SELECT_COMMAND, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
