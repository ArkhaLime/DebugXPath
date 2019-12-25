using System;

namespace DebugXPath.Helpers
{
    //inspired from https://github.com/colored-console/colored-console/blob/dev/src/ColoredConsole/ColorConsole.cs
    public static class ColoredConsole
    {

        public static void Write(string value, ConsoleColor? forecolor = null, ConsoleColor? backcolor = null)
        {

            if (forecolor.HasValue || backcolor.HasValue)
            {
                var originalColor = Console.ForegroundColor;
                var originalBackgroundColor = Console.BackgroundColor;
                try
                {
                    Console.ForegroundColor = forecolor ?? originalColor;
                    Console.BackgroundColor = backcolor ?? originalBackgroundColor;
                    Console.Write(value);
                }
                finally
                {
                    Console.ForegroundColor = originalColor;
                    Console.BackgroundColor = originalBackgroundColor;
                }
            }
            else
            {
                Console.Write(value);
            }
        }

        public static void Write(string value)
        {
            Console.Write(value);
        }

        public static void WriteLine(string value, ConsoleColor? forecolor = null, ConsoleColor? backcolor = null)
        {
            Write(value, forecolor, backcolor);
            Console.WriteLine();
        }

        public static void WriteLine(string value)
        {
            Console.WriteLine(value);
        }

    }
}
