using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PassonProject.Interfaces;  // Import the interface for PlaylistXSongService
using PassonProject.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PassonProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaylistXSongsController : ControllerBase
    {
        private readonly IPlaylistXSongService _playlistXSongService;

        // Constructor to inject IPlaylistXSongService
        public PlaylistXSongsController(IPlaylistXSongService playlistXSongService)
        {
            _playlistXSongService = playlistXSongService;
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
            var songs = await _playlistXSongService.GetSongsByPlaylistIdAsync(playlistId);

            if (songs == null || !songs.Any())
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
            var playlists = await _playlistXSongService.GetPlaylistsBySongIdAsync(songId);

            if (playlists == null || !playlists.Any())
            {
                return NotFound($"No playlists found for song with ID {songId}.");
            }

            return Ok(playlists);
        }

        /// <summary>
        /// Adds a song to a playlist.
        /// </summary>
        /// <param name="playlistId">The ID of the playlist to add the song to.</param>
        /// <param name="songId">The ID of the song to add.</param>
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
        public async Task<IActionResult> AddSongToPlaylist(int playlistId, int songId)
        {
            if (playlistId == 0 || songId == 0)
            {
                return BadRequest("Invalid playlist or song ID.");
            }

            // Call the service with both playlistId and songId
            var result = await _playlistXSongService.AddSongToPlaylistAsync(playlistId, songId);

            if (result)
            {
                // Successfully added the song to the playlist
                return CreatedAtAction(nameof(ListSongsForPlaylist), new { playlistId = playlistId }, new { playlistId, songId });
            }
            else
            {
                // If the song was already in the playlist or any other issue
                return BadRequest("Failed to add song to playlist.");
            }
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
            var result = await _playlistXSongService.RemoveSongFromPlaylistAsync(playlistId, songId);

            if (!result)
            {
                return NotFound($"Song with ID {songId} not found in playlist with ID {playlistId}.");
            }

            return Ok(new { message = "Song removed from playlist successfully", playlistId, songId });
        }
    }
}
