using System.Security.Principal;
using PamelloClientCLI.Enumerators;
using PamelloV7.Wrapper;
using PamelloV7.Wrapper.Model;

namespace PamelloClientCLI;

/*

command: -c [command name] [command args, ...]
alias: -a [from] [to]
    if {from} is "remove", remove alias at {to} position
data: -d [repo] [value]
server: -s [address] [token]
help: -h

*/

class Program
{
    public ECommand Command;
    public string FirstArg;
    public string[] SecondArgs;

    private readonly SavedInfo _savedInfo;
    private readonly PamelloClient _pamello;

    private readonly Commands _commands;

    public Program() {
        _savedInfo = new SavedInfo();
        _pamello = new PamelloClient();
        
        _commands = new Commands(_savedInfo, _pamello);
    }
    
    public static Task Main(string[] args) => new Program().MainAsync(args);
    
    private async Task MainAsync(string[] args) {
        if (!ParseArgs(args)) return;
        _savedInfo.Load();

        switch (Command)
        {
            case ECommand.Server:
            {
                var tokencode = SecondArgs.ElementAtOrDefault(0);
                if (tokencode is null) {
                    Console.WriteLine("token/code is required");
                    return;
                }
                
                await _commands.Server(FirstArg, tokencode);
            } break;
            case ECommand.Data: await _commands.Data(FirstArg, SecondArgs[0]); break; 
        }
    }

    public bool ParseArgs(string[] args) {
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
                Console.WriteLine("At least one argument is required");
                return false;
            }
            FirstArg = args[1];

            if (args[0][1] == 'd') {
                SecondArgs = new string[1];
                SecondArgs[0] = args.ElementAtOrDefault(2) ?? "current";
                Command = ECommand.Data;
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
                    Console.WriteLine($"Unknown '{args[0]}' command");
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