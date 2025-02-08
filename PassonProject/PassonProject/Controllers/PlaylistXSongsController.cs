using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PassonProject.Data;
using PassonProject.Models;

namespace PassonProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaylistXSongsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public PlaylistXSongsController(ApplicationDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="playlistId"></param>
        /// <returns></returns>
        // Get all songs in a playlist
        [Route("ListSongsForPlaylist/{playlistId}")]
        [HttpGet]
        public async Task<IActionResult> ListSongsForPlaylist(int playlistId)
        {
            var songs = await _context.PlaylistXSong
                .Where(ps => ps.PlaylistId == playlistId)
                .Select(ps => new PlaylistXSongDTO
                {
                    PlaylistId = ps.PlaylistId,
                    SongId = ps.SongId,
                    AddedDate = ps.AddedDate
                })
                .ToListAsync();

            if (!songs.Any())
            {
                return NotFound($"No songs found for playlist with ID {playlistId}.");
            }

            return Ok(songs);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="songId"></param>
        /// <returns></returns>
        // Get all playlists containing a specific song
        [Route("ListPlaylistsForSong/{songId}")]
        [HttpGet]
        public async Task<IActionResult> ListPlaylistsForSong(int songId)
        {
            var playlists = await _context.PlaylistXSong
                .Where(ps => ps.SongId == songId)
                .Select(ps => new PlaylistXSongDTO
                {
                    PlaylistId = ps.PlaylistId,
                    SongId = ps.SongId,
                    AddedDate = ps.AddedDate
                })
                .ToListAsync();

            if (!playlists.Any())
            {
                return NotFound($"No playlists found for song with ID {songId}.");
            }

            return Ok(playlists);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="playlistXSongDTO"></param>
        /// <returns></returns>
        // Add a song to a playlist
        [Route("AddSongToPlaylist")]
        [HttpPost]
        public async Task<IActionResult> AddSongToPlaylist(PlaylistXSongDTO playlistXSongDTO)
        {
            if (playlistXSongDTO == null)
            {
                return BadRequest("Invalid playlist-song data.");
            }

            var playlistXSongEntity = new PlaylistXSong
            {
                PlaylistId = playlistXSongDTO.PlaylistId,
                SongId = playlistXSongDTO.SongId,
                AddedDate = playlistXSongDTO.AddedDate
            };

            _context.PlaylistXSong.Add(playlistXSongEntity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ListSongsForPlaylist), new { playlistId = playlistXSongDTO.PlaylistId }, playlistXSongDTO);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="playlistId"></param>
        /// <param name="songId"></param>
        /// <returns></returns>
        // Remove a song from a playlist
        [Route("RemoveSongFromPlaylist/{playlistId}/{songId}")]
        [HttpDelete]
        public async Task<IActionResult> RemoveSongFromPlaylist(int playlistId, int songId)
        {
            var playlistXSongEntity = await _context.PlaylistXSong
                .FirstOrDefaultAsync(ps => ps.PlaylistId == playlistId && ps.SongId == songId);

            if (playlistXSongEntity == null)
            {
                return NotFound($"Song with ID {songId} not found in playlist with ID {playlistId}.");
            }

            _context.PlaylistXSong.Remove(playlistXSongEntity);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="playlistXSongDTO"></param>
        /// <returns></returns>
        // Update the date a song was added to a playlist
        [Route("UpdateSongAddedDate")]
        [HttpPut]
        public async Task<IActionResult> UpdateSongAddedDate(PlaylistXSongDTO playlistXSongDTO)
        {
            var playlistXSongEntity = await _context.PlaylistXSong
                .FirstOrDefaultAsync(ps => ps.PlaylistId == playlistXSongDTO.PlaylistId && ps.SongId == playlistXSongDTO.SongId);

            if (playlistXSongEntity == null)
            {
                return NotFound($"Song with ID {playlistXSongDTO.SongId} not found in playlist with ID {playlistXSongDTO.PlaylistId}.");
            }

            playlistXSongEntity.AddedDate = playlistXSongDTO.AddedDate;
            _context.Entry(playlistXSongEntity).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
