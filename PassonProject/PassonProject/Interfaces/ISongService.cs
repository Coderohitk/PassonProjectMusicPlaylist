using PassonProject.Models;
namespace PassonProject.Interfaces
{
    public interface ISongService
    {
        Task<IEnumerable<SongDTO>> ListSongsAsync();
        Task<SongDTO> FindSongAsync(int id);
        Task<SongDTO> AddSongAsync(SongDTO songDto);
        Task<SongDTO> UpdateSongAsync(int id, SongDTO songDto);
        Task<bool> DeleteSongAsync(int id);
    }
}
