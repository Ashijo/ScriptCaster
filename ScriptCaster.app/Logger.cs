using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScriptCaster.app
{
    public static class Logger
    {
        /* Should logger store and manage errors ?
        The idea may seem weird, it can be seen as a double purpose class
        But the only things that errors does in this context is log and explain the problem
        Finally in this app, at least for now, error are only la list of message to prompt
        Maybe errors will do more stuff later, rollback service could be add
        At this time a refactoring will be mandatory
        */
        
        public static void Log(string message, ConsoleColor color) {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void LogWarning (string message) {
            Log(message, ConsoleColor.Yellow);
        }
        public static void LogError (string message){
            Log(message, ConsoleColor.Red);
        }
    }
}