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

    public readonly SavedInfo SavedInfo;
    public readonly PamelloClient Pamello;

    public Program() {
        SavedInfo = new SavedInfo();
        Pamello = new PamelloClient();
    }
    
    public static Task Main(string[] args) => new Program().MainAsync(args);
    
    private async Task MainAsync(string[] args) {
        if (!ParseArgs(args)) return;
        SavedInfo.Load();

        switch (Command)
        {
            case ECommand.Server: await Server(FirstArg, SecondArgs[0]); break; 
            case ECommand.Data: await Data(FirstArg, SecondArgs[0]); break; 
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
                    Console.WriteLine($"Unknown '{argPosition}' command");
                    return false;
                }
            }
        }

        return true;
    }

    public async Task<bool> CommandAuthorize()
    {
        Pamello.ServerHost = SavedInfo.ServerAddress;
        await Pamello.Authorization.WithTokenAsync(SavedInfo.Token!.Value);
        return true;
    }
    
    public void WriteHelp(ECommand? command = null) {
        Console.WriteLine("helop");
    }
    
    public async Task Server(string address, string tokencode)
    {   
        Pamello.ServerHost = address;

        if (int.TryParse(tokencode, out var code))
        {
            await Pamello.Authorization.WithCodeAsync(code);
        }
        else if (Guid.TryParse(tokencode, out var token))
        {
            await Pamello.Authorization.WithTokenAsync(token);
        }

        SavedInfo.ServerAddress = Pamello.ServerHost;
        SavedInfo.Token = Pamello.Authorization.UserToken;
        SavedInfo.Save();
    }

    public void Alias()
    {
        
    }

    public async Task Data(string repo, string value)
    {
        await CommandAuthorize();
        
        switch (repo.ToLower())
        {
            case "user":
            {
                var user = await Pamello.Users.GetNew(value);
                Console.WriteLine($"Name: {user.Name}");
                break;
            }
            case "player":
            {
                var player = await Pamello.Players.GetNew(value);;
                Console.WriteLine($"Name: {player.Name}");
                break;
            }
            case "episode":
            {
                var episode = await Pamello.Episodes.GetNew(value);
                Console.WriteLine($"Name: {episode.Name}");
                break;
            }
            case "song":
            {
                var song = await Pamello.Songs.GetNew(value);
                Console.WriteLine($"Name: {song.Name}");
                break;
            }
        }
    }
    
}