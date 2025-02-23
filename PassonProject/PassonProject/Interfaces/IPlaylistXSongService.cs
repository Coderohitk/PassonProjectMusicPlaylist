using System.Collections.Generic;
using System.Threading.Tasks;
using PassonProject.Models;

namespace PassonProject.Interfaces
{
    public interface IPlaylistXSongService
    {
        Task<IEnumerable<PlaylistXSongDTO>> GetSongsByPlaylistIdAsync(int playlistId);
        Task<IEnumerable<PlaylistXSongDTO>> GetPlaylistsBySongIdAsync(int songId);

        Task<bool> AddSongToPlaylistAsync(int playlistId, int songId);
        Task<bool> RemoveSongFromPlaylistAsync(int playlistId, int songId);

        // Methods for adding and removing playlists from songs
        Task<bool> AddPlaylistToSongAsync(int songId, int playlistId);
        Task<bool> RemovePlaylistFromSongAsync(int songId, int playlistId);
    }
}
