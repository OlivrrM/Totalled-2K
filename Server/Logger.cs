using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logger
{
    public static class ConsoleLogger
    {
        public static void LogError(string error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[ERROR]: " + error);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static void LogWarning(string warning)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[WARNING]: " + warning);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static void LogInfo(string info)
        {
            Console.WriteLine("[INFO]: " + info);
        }
    }
}