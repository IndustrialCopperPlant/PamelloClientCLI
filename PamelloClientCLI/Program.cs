using PamelloClientCLI.Enumerators;

namespace PamelloClientCLI;

/*

command: -c [command name] [command args, ...]
alias: -a [from] [to]
    if {from} is "remove", remove alias at {to} position
data: -d [repo] [value]
server: -s [address]
help: -h

*/

class Program
{
    public ECommand Command;
    public string FirstArg;
    public string[] SecondArgs;
    
    public static Task Main(string[] args) => new Program().MainAsync(args);
    
    private async Task MainAsync(string[] args) {
        if (!ParseArgs(args)) return;

        Console.WriteLine($"Command: {Command}");
        Console.WriteLine($"First aRh: {FirstArg}");
        Console.WriteLine("Second args:");
        foreach (var arg in SecondArgs) {
            Console.WriteLine(arg);
        }
    }

    public bool ParseArgs(string[] args) {
        int argPosition = 0;

        if (args.Length == 0 || args[0] == "-h") {
            WriteHelp();
            return false;
        }
        
        if (args[0][0] != '-') {
            Command = ECommand.Command;
            FirstArg = args[0];
            
            SecondArgs = new string[args.Length - 1];
            for (int i = 0; i < SecondArgs.Length; i++) {
                SecondArgs[i] = args[i + 1];
            }
        }
        else {
            if (args.Length < 2) {
                Console.WriteLine("yeblan");
                return false;
            }
            FirstArg = args[1];

            if (args[0][1] == 'd') {
                SecondArgs = new string[1];
                SecondArgs[0] = args.ElementAtOrDefault(2) ?? "current";
                return true;
            }
            
            SecondArgs = new string[args.Length - 2];
            for (int i = 0; i < SecondArgs.Length; i++) {
                SecondArgs[i] = args[i + 2];
            }
            
            switch (args[0][1]) {
                case 'c': Command = ECommand.Command; break;
                case 'a': Command = ECommand.Alias; break;
                case 'd': Command = ECommand.Data; break;
                case 's': Command = ECommand.Server; break;
                default: {
                    Console.WriteLine($"Unknown '{argPosition}' command");
                    return false;
                }
            }
        }

        return true;
    }

    public void WriteHelp(ECommand? command = null) {
        Console.WriteLine("helop");
    }
}