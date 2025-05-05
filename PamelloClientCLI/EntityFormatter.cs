using System.Text;
using PamelloV7.Core.Audio;
using PamelloV7.Core.DTO;
using PamelloV7.Wrapper;
using PamelloV7.Wrapper.Model;

namespace PamelloClientCLI;

public class EntityFormatter
{
    private readonly PamelloClient _pamello;

    public EntityFormatter(PamelloClient pamello) {
        _pamello = pamello;
    }
    
    public async Task<string> ToString(object entity, bool isShort) {
        if (entity is RemoteSong song) return isShort ? SongToShortString(song) : await SongToString(song);
        if (entity is RemotePlayer player) return isShort ? PlayerToShortString(player) : await PlayerToString(player);
        if (entity is RemoteUser user) return isShort ? UserToShortString(user) : await UserToString(user);
        if (entity is RemoteEpisode episode) return isShort ? EpisodeToShortString(episode) : await EpisodeToString(episode);
    
        throw new Exception("Unrecognized entity");
    }

    public static string SongToShortString(RemoteSong? song) {
        return song is null ? "None" : $"{song.Name} [{song.Id}]";
    }
    
    public static string PlayerToShortString(RemotePlayer? player) {
        return player is null ? "None" : $"{player.Name} [{player.Id}]";
    }
    
    public static string UserToShortString(RemoteUser? user) {
        return user is null ? "None" : $"{user.Name} [{user.Id}]";
    }
    
    public static string EpisodeToShortString(RemoteEpisode? episode) {
        return episode is null ? "None" : $"{episode.Name} [{episode.Id}]";
    }
    
    public async Task<string> SongToString(RemoteSong song) {
        return @$"--- Song {song.Id} ---
{song.Name}
---
Added by: {UserToShortString(await _pamello.Users.Get(song.AddedById))}
Favorite by: {song.FavoriteByIds.Count()} Users
Playcount: {song.PlayCount}
Added at: {song.AddedAt}
";
    }
    
    public async Task<string> PlayerToString(RemotePlayer player) {
        var queueSb = new StringBuilder();

        RemoteSong? queueSong;
        foreach (var queueEntry in player.QueueEntriesDTOs) {
            queueSong = await _pamello.Songs.Get(queueEntry.SongId);
            queueSb.Append("    ");
            queueSb.AppendLine(SongToShortString(queueSong));
        }

        if (queueSb.Length == 0) queueSb.Append("    Empty");
        
        return @$"--- Player {player.Id} ---
{player.Name}
---
State: {player.State}
Current Song: {(player.CurrentSongId is not null ? SongToShortString(await _pamello.Songs.Get(player.CurrentSongId.Value)) : "None")}
Paused: {(player.IsPaused ? "Yes" : "No")}
Modes:
    Reversed: {(player.QueueIsReversed ? "Yes" : "No")}
    Random: {(player.QueueIsRandom ? "Yes" : "No")}
    No Leftovers: {(player.QueueIsNoLeftovers ? "Yes" : "No")}
    Feed Random: {(player.QueueIsFeedRandom ? "Yes" : "No")}
Queue:
{queueSb}";
    }
    
    public async Task<string> UserToString(RemoteUser user) {
        return @$"--- User {user.Id} - {user.DiscordId} ---
{user.Name}
---
Selected Player: {PlayerToShortString(await _pamello.Players.Get(user.SelectedPlayerId ?? -1))}
Songs Added: {user.AddedSongsIds.Count()}
Songs Played: {user.SongsPlayed}";
    }
    
    public async Task<string> EpisodeToString(RemoteEpisode episode) {
        return @$"--- Episode {episode.Id} ---
{new AudioTime(episode.Start).ToShortString()} - {episode.Name}
---
Song: {SongToShortString(await _pamello.Songs.Get(episode.SongId))}
Auto Skip: {(episode.Skip ? "Yes" : "No")}
";
    }
}