using PamelloClientCLI.Enumerators;
using PamelloV7.Core.Exceptions;
using PamelloV7.Wrapper;

namespace PamelloClientCLI;

public class Commands
{
    private readonly SavedInfo _savedInfo;
    private readonly PamelloClient _pamello;

    public Commands(SavedInfo savedInfo, PamelloClient pamello) {
        _savedInfo = savedInfo;
        _pamello = pamello;
    }
    
    public async Task<bool> Authorize()
    {
        if (_savedInfo.ServerAddress is null) return false;
        if (_savedInfo.Token is null) return false;
        
        _pamello.ServerHost = _savedInfo.ServerAddress;
        await _pamello.Authorization.WithTokenAsync(_savedInfo.Token.Value, false);
        
        return _pamello.Users.Current is not null;
    }
    
    public async Task Server(string address, string tokencode)
    {
        try {
            if (!await _pamello.Events.Connect(address)) {
                Console.WriteLine($"Cant connect to \"{address}\"");
                return;
            }
        }
        catch {
            Console.WriteLine($"Cant connect to \"{address}\"");
            return;
        }

        if (int.TryParse(tokencode, out var code))
        {
            if (!await _pamello.Authorization.WithCodeAsync(code)) {
                Console.WriteLine($"Code \"{code}\" is invalid");
                return;
            }
        }
        else if (Guid.TryParse(tokencode, out var token))
        {
            try {
                await _pamello.Authorization.WithTokenAsync(token);
            }
            catch (PamelloException) {
                Console.WriteLine($"Token \"{token}\" is invalid");
                return;
            }
        }

        _savedInfo.ServerAddress = _pamello.ServerHost;
        _savedInfo.Token = _pamello.Authorization.UserToken;
        _savedInfo.Save();
    }

    public void Alias()
    {
        
    }

    public async Task Data(string repo, string value)
    {
        if (!await Authorize()) {
            Console.WriteLine("You need to me authorized to use this command, authorize using -s [address] [token]");
            return;
        }
        
        switch (repo.ToLower())
        {
            case "user":
            {
                var user = await _pamello.Users.GetNew(value);
                if (user is null) {
                    Console.WriteLine($"User with value \"{value}\" not found");
                    return;
                }
                
                /*
                
                Marsoau [1 - 100000000]
                Songs Added: 10
                Songs Played: 5
                 
                */
                
                Console.WriteLine($"{user.Name} [{user.Id} - {user.DiscordId}]");
                Console.WriteLine($"Songs Added: {user.AddedSongsIds.Count()}");
                Console.WriteLine($"Songs Played: {user.SongsPlayed}");
                break;
            }
            case "player":
            {
                var player = await _pamello.Players.GetNew(value);;
                if (player is null) {
                    Console.WriteLine($"User with value \"{value}\" not found");
                    return;
                }
                Console.WriteLine($"Name: {player.Name}");
                break;
            }
            case "episode":
            {
                var episode = await _pamello.Episodes.GetNew(value);
                if (episode is null) {
                    Console.WriteLine($"User with value \"{value}\" not found");
                    return;
                }
                Console.WriteLine($"Name: {episode.Name}");
                break;
            }
            case "song":
            {
                var song = await _pamello.Songs.GetNew(value);
                if (song is null) {
                    Console.WriteLine($"User with value \"{value}\" not found");
                    return;
                }
                
                Console.WriteLine($"Name: {song.Name}");
                break;
            }
        }
    }
}