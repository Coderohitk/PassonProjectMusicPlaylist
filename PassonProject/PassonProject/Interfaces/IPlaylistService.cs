using System.Collections.Generic;
using System.Threading.Tasks;
using PassonProject.Models;

namespace PassonProject.Interfaces
{
    public interface IPlaylistService
    {
        Task<IEnumerable<PlaylistDTO>> GetAllPlaylistsAsync();
        Task<PlaylistDTO> GetPlaylistByIdAsync(int id);
        Task<PlaylistDTO> AddPlaylistAsync(PlaylistDTO playlistDTO);
        Task<bool> UpdatePlaylistAsync(int id, PlaylistDTO playlistDTO);
        Task<bool> DeletePlaylistAsync(int id);
    }
}
