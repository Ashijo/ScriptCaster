
using ScriptCaster.app;

class Program {

    static void Main(string[] args) { 
        Console.WriteLine("Welcome into Script Caster :)");

        if(args.Length < 1) {
            Console.WriteLine(" You need to tell me which template I shall use :( ");
            return;
        }

        var templateName = args[0];
        Context.Instance.InitContext(templateName);

        Cast.LaunchCast();

    }
}