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
        /// Retrieves all songs in a specific playlist.
        /// </summary>
        /// <param name="playlistId">The ID of the playlist to retrieve songs from.</param>
        /// <returns>A list of songs in the specified playlist.</returns>
        /// <response code="200">Returns the list of songs in the playlist.</response>
        /// <response code="404">If the playlist contains no songs.</response>
        /// <example>
        /// GET: api/PlaylistXSongs/ListSongsForPlaylist/3
        /// </example>
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
        /// Retrieves all playlists containing a specific song.
        /// </summary>
        /// <param name="songId">The ID of the song to find playlists for.</param>
        /// <returns>A list of playlists that contain the specified song.</returns>
        /// <response code="200">Returns the list of playlists that contain the song.</response>
        /// <response code="404">If the song is not found in any playlist.</response>
        /// <example>
        /// GET: api/PlaylistXSongs/ListPlaylistsForSong/2
        /// </example>
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
        /// Adds a song to a playlist.
        /// </summary>
        /// <param name="playlistXSongDTO">An object containing the PlaylistId, SongId, and the date the song was added.</param>
        /// <returns>The newly added song entry.</returns>
        /// <response code="201">Returns the added song entry.</response>
        /// <response code="400">If the provided data is invalid.</response>
        /// <example>
        /// POST: api/PlaylistXSongs/AddSongToPlaylist
        /// Body:
        /// {
        ///   "playlistId": 3,
        ///   "songId": 2,
        ///   "addedDate": "2025-02-08T18:35:08.126Z"
        /// }
        /// </example>
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
        /// Removes a song from a playlist.
        /// </summary>
        /// <param name="playlistId">The ID of the playlist from which to remove the song.</param>
        /// <param name="songId">The ID of the song to remove.</param>
        /// <returns>A confirmation message if the song was successfully removed.</returns>
        /// <response code="200">If the song was successfully removed.</response>
        /// <response code="404">If the song was not found in the playlist.</response>
        /// <example>
        /// DELETE: api/PlaylistXSongs/RemoveSongFromPlaylist/3/2
        /// </example>
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

            return Ok(new
            {
                message = "Song removed from playlist successfully",
                playlistId = playlistId,
                songId = songId
            });
        }
    }
}
