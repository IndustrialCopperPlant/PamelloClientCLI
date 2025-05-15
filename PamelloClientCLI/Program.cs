using System.Security.Principal;
using System.Text;
using PamelloClientCLI.Commands;
using PamelloClientCLI.Enumerators;
using PamelloV7.Wrapper;
using PamelloV7.Wrapper.Model;

namespace PamelloClientCLI;

/*
*/

class Program
{
    public ECommandName CommandName;
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
        Console.InputEncoding = Encoding.Unicode;
        Console.OutputEncoding = Encoding.Unicode;
        
        if (!ParseArgs(args)) return;
        _savedInfo.Load();

        _pamello.ServerHost = "127.0.0.1:51630";
        await _pamello.Authorization.WithTokenAsync(Guid.Parse("D01E6353-2EC7-469C-81A5-D3084FB17151"));

        var command = new DataCommand(_pamello);
        command.Repository = ERepositoryName.Player;
        command.EntityValue = "current";
        
        await command.Execute();

        //WriteHelp();
    }

    public bool ParseArgs(string[] args) {
        return true;
    }

    public void WriteHelp(ECommandName? command = null) {
        Console.WriteLine(@"Help:

-A, --authorize <server host> <code | token>
    <server host>: host of server in ip:port format. if port is not provided, default 51630 port will be used
    <code | token>: code or token that will be used for authorization
    
    if only one argument is provided, it will be used as code | token on last used server


-c, --command <command> [args...]
    <command>: name of the command that will be executed
    [args...]: arguments that will be passed to command
    
    allows to execute command on server

    if no arguments are provided, list all commands


-a, --alias <alias from> <to command>
    <alias from>: alias that will be used to execute command
    <to command>: command that will be executed
    
    create an alias to command name (only name, for arguments use macro)

    if no arguments are provided, list all aliases


-m, --macro <name> [command > command > ...]
    <name>: name of the macro
    command: command in format: <command> [args...]

    create macro to execute multiple commands with arguments in sequence

    if no arguments are provided, list all macros

-r, --remove <name>
    <name>: name of the macro or alias to remove

    remove macro or alias


-d, --data <repository> <value>
    <repository>: player | user | song | episode
    <value>: value of entity in repository, like id, name, etc.
    
    get data of entity in repository


-s, --search <repository> <query> <page>
    <repository>: player | user | song | episode
    <query>: query that will be used for search
    <page>: page of search results (optional, default 1)

    search for entities in repository
    
    
-h, --help
    display this help message


Examples:


Authorization:
    pamello -A 127.0.0.1 343121
    pamello -A 127.0.0.1 00c977a6-22c5-429f-af62-46e830f03dc1

Command:
    pamello -c Skip
    pamello -c SongFavoriteAdd current

Alias:
    pamello -a sfa SongFavoriteAdd
    pamello -a ps PlayerSelect

    So later you can use:
        pamello ps 1
        pamello sfa current

Macro:
    pamello -m sfac SongFavoriteAdd current
    pamello -m init SpeakerConnect > PlayerQueueFeedRandom true

Data:
    pamello -d player
    pamello -d user
    pamello -d user 3
    pamello -d song 1000

Search:
    pamello -s players
    pamello -s songs ""artur pirozhkov""
");
    }
}
