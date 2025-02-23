using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PassonProject.Interfaces;
using PassonProject.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PassonProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaylistsController : ControllerBase
    {
        private readonly IPlaylistService _playlistService;

        public PlaylistsController(IPlaylistService playlistService)
        {
            _playlistService = playlistService;
        }

        /// <summary>
        /// Retrieves a list of all playlists.
        /// </summary>
        /// <returns>A list of playlists.</returns>
        /// <response code="200">Returns the list of playlists.</response>
        /// <example>
        /// GET: api/Playlists/ListPlaylists
        /// </example>
        [Route("ListPlaylists")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlaylistDTO>>> ListPlaylists()
        {
            var playlists = await _playlistService.GetAllPlaylistsAsync();
            return Ok(playlists);
        }

        /// <summary>
        /// Retrieves a specific playlist by ID.
        /// </summary>
        /// <param name="id">Playlist ID.</param>
        /// <returns>The requested playlist.</returns>
        /// <response code="200">Returns the requested playlist.</response>
        /// <response code="404">If the playlist is not found.</response>
        /// <example>
        /// GET: api/Playlists/FindPlaylist/1
        /// </example>
        [Route("FindPlaylist/{id}")]
        [HttpGet]
        public async Task<ActionResult<PlaylistDTO>> FindPlaylist(int id)
        {
            var playlistDTO = await _playlistService.GetPlaylistByIdAsync(id);
            if (playlistDTO == null)
            {
                return NotFound();
            }
            return Ok(playlistDTO);
        }

        /// <summary>
        /// Adds a new playlist.
        /// </summary>
        /// <param name="playlistDTO">The playlist object to add.</param>
        /// <returns>The created playlist.</returns>
        /// <response code="201">Returns the created playlist.</response>
        /// <response code="400">If the request is invalid.</response>
        /// <example>
        /// POST: api/Playlists/AddPlaylist
        /// Body:
        /// {
        ///     "PlaylistName": "Chill Vibes",
        ///     "PlaylistDescription": "A playlist for relaxing music",
        ///     "CreatedAt": "2025-02-08T14:00:00Z"
        /// }
        /// </example>
        [Route("AddPlaylist")]
        [HttpPost]
        public async Task<ActionResult<PlaylistDTO>> AddPlaylist(PlaylistDTO playlistDTO)
        {
            if (playlistDTO == null)
            {
                return BadRequest("Invalid playlist data.");
            }

            var createdPlaylist = await _playlistService.AddPlaylistAsync(playlistDTO);
            return CreatedAtAction(nameof(FindPlaylist), new { id = createdPlaylist.PlaylistId }, createdPlaylist);
        }

        /// <summary>
        /// Updates an existing playlist.
        /// </summary>
        /// <param name="id">The ID of the playlist to update.</param>
        /// <param name="playlistDTO">Updated playlist details.</param>
        /// <returns>A success message.</returns>
        /// <response code="200">If the playlist is updated successfully.</response>
        /// <response code="400">If the request is invalid.</response>
        /// <response code="404">If the playlist is not found.</response>
        /// <example>
        /// PUT: api/Playlists/UpdatePlaylist/1
        /// Body:
        /// {
        ///     "PlaylistId": 1,
        ///     "PlaylistName": "Updated Name",
        ///     "PlaylistDescription": "Updated Description",
        ///     "CreatedAt": "2025-02-08T14:00:00Z"
        /// }
        /// </example>
        [Route("UpdatePlaylist/{id}")]
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdatePlaylist(int id, PlaylistDTO playlistDTO)
        {
            if (id != playlistDTO.PlaylistId)
            {
                return BadRequest();
            }

            var updated = await _playlistService.UpdatePlaylistAsync(id, playlistDTO);
            if (!updated)
            {
                return NotFound();
            }

            return Ok(new { message = "Playlist updated successfully", playlist = playlistDTO });
        }

        /// <summary>
        /// Deletes a playlist by ID.
        /// </summary>
        /// <param name="id">Playlist ID.</param>
        /// <returns>A success message.</returns>
        /// <response code="200">If the playlist is deleted successfully.</response>
        /// <response code="404">If the playlist is not found.</response>
        /// <example>
        /// DELETE: api/Playlists/DeletePlaylist/1
        /// </example>
        [Route("DeletePlaylist/{id}")]
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeletePlaylist(int id)
        {
            var deleted = await _playlistService.DeletePlaylistAsync(id);
            if (!deleted)
            {
                return NotFound($"Playlist with ID {id} not found.");
            }

            return Ok(new { message = "Playlist deleted successfully", playlistId = id });
        }
    }
}
