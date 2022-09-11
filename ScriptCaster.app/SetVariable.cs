
namespace ScriptCaster.app
{

    /*
    * Set variable class shall alow me to edit the .variables.json files
    * Depending on the option, either editing the global variables or the variable of the template
    */
    public static class SetVariable
    {
        public static void LaunchVariableSetting(VariableFile variableFile) {
            Console.WriteLine("Variable setting");

            if(variableFile.HasFlag(VariableFile.Global)) {
                Console.WriteLine("Global variables (variables common to all templates) :");
                ReadInput(OptionsDisplay(), VariableFile.Global);
            }

            if(variableFile.HasFlag(VariableFile.Template)) {
                Console.WriteLine("Template variables (variables of the template) :");
                ReadInput(OptionsDisplay(), VariableFile.Template);
            }

        }

        private static string[] OptionsDisplay(){
            string[]? input = {};
            bool first = true;
            do {
                if(!first) {
                    Console.WriteLine("Not a valid input, please retry :");
                }

                Logger.Log("l|L|list list all variables", ConsoleColor.Cyan);
                Logger.Log("a|A|add add a new variable", ConsoleColor.Cyan);
                Logger.Log("r|R|remove <VariableName> remove a variable", ConsoleColor.Cyan);
                Logger.Log("e|E|edit <VariableName> edit a variable", ConsoleColor.Cyan);
                Logger.Log("ea|EA|editAll edit all variables", ConsoleColor.Cyan);
                Logger.Log("q|Q|quit leave", ConsoleColor.Cyan);
            
                input = Console.ReadLine()?.Split(' ');
            
            } while (input == null || input.Length == 0);
            
            return input;
        } 

        private static void ReadInput(string[] cmd, VariableFile target) {
            switch (cmd[0]) {
                case "l": 
                case "L": 
                case "list": 
                    List(target);
                break;
                case "a":
                case "A":
                case "add":
                    Add();
                break;
                case "r":
                case "R":
                case "remove":
                    Remove();
                break;
                case "e":
                case "E":
                case "edit":
                    Edit();
                break;
                case "q":
                case "Q":
                case "quit":
                return;
                default:
                    Console.WriteLine("input not understood");
                    return;
            }
        }

        private static void List(VariableFile target){}
        private static void Add() {}
        private static void Remove() {}
        private static void Edit() {}
        private static void EditAll() {}

    }
}