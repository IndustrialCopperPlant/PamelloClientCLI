using PamelloClientCLI.Enumerators;
using PamelloV7.Wrapper;

namespace PamelloClientCLI.Commands;

public class DataCommand : Command
{
    public ERepositoryName Repository { get; set; }
    public string EntityValue { get; set; }

    private readonly EntityFormatter _formatter;
    
    public DataCommand(PamelloClient pamello) : base(ECommandName.Data, pamello) {
        _formatter = new EntityFormatter(pamello);
    }

    public override void InitFromArgs() {
        if (Args.Count == 0) throw new Exception("Not enough arguments");

        switch (Args[0].ToLower()) {
            case "player": Repository = ERepositoryName.Player; break;
            case "user": Repository = ERepositoryName.User; break;
            case "song": Repository = ERepositoryName.Song; break;
            case "episode": Repository = ERepositoryName.Episode; break;
            default: throw new Exception("Unknown repository");
        }
        
        EntityValue = Args.Count > 1 ? Args[1] : "current";
    }

    public override async Task Execute() {
        object? entity = null;
        
        switch (Repository) {
            case ERepositoryName.Player:
            {
                entity = await _pamello.Players.GetNew(EntityValue);
            } break;
            case ERepositoryName.User:
            {
                entity = await _pamello.Users.GetNew(EntityValue);
            } break;
            case ERepositoryName.Song:
            {
                entity = await _pamello.Songs.GetNew(EntityValue);
            } break;
            case ERepositoryName.Episode:
            {
                entity = await _pamello.Episodes.GetNew(EntityValue);
            } break;
            default: throw new Exception("Unknown repository");
        }

        if (entity is null) {
            Console.WriteLine($"Entity with value \"{EntityValue}\" not found");
            return;
        }
        
        Console.WriteLine(await _formatter.ToString(entity, false));
    }

    public override string Help() {
        throw new NotImplementedException();
    }

    public override string Examples() {
        throw new NotImplementedException();
    }
}