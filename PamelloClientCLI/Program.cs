using System.Security.Principal;
using PamelloClientCLI.Commands;
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

    private readonly PamelloClient _pamello;
    private readonly SavedInfo _savedInfo;

    private readonly Queue<Command> _commands;

    public Program() {
        _pamello = new PamelloClient();
        _savedInfo = new SavedInfo();

        _commands = new Queue<Command>();
    }
    
    public static Task Main(string[] args) => new Program().MainAsync(args);
    
    private async Task MainAsync(string[] args) {
        if (!ParseArgs(args)) return;
        _savedInfo.Load();
    }

    public bool ParseArgs(string[] args) {
        return false;
    }

    public void WriteHelp(ECommand? command = null) {
        Console.WriteLine("helop");
    }
}