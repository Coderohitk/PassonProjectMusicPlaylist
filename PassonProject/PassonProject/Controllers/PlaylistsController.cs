using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PassonProject.Data;
using PassonProject.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PassonProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaylistsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public PlaylistsController(ApplicationDbContext context)
        {
            _context = context;
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
            var playlists = await _context.Playlists.ToListAsync();
            var PlaylistDTOS = playlists.Select(p => new PlaylistDTO
            {
                PlaylistId = p.PlaylistId,
                PlaylistName = p.PlaylistName,
                PlaylistDescription = p.PlaylistDescription,
                CreatedAt = p.CreatedAt,
            }).ToList();
            return Ok(PlaylistDTOS);
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
            var playlistEntity = await _context.Playlists.FindAsync(id);
            if (playlistEntity == null)
            {
                return NotFound();
            }
            var playlistDTO = new PlaylistDTO
            {
                PlaylistId = playlistEntity.PlaylistId,
                PlaylistName = playlistEntity.PlaylistName,
                PlaylistDescription = playlistEntity.PlaylistDescription,
                CreatedAt = playlistEntity.CreatedAt,
            };
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
            var playlistEntity = new Playlist
            {
                PlaylistName = playlistDTO.PlaylistName,
                PlaylistDescription = playlistDTO.PlaylistDescription,
                CreatedAt = playlistDTO.CreatedAt,
            };
            _context.Playlists.Add(playlistEntity);
            await _context.SaveChangesAsync();
            playlistDTO.PlaylistId = playlistEntity.PlaylistId;
            return CreatedAtAction(nameof(FindPlaylist), new { id = playlistEntity.PlaylistId }, playlistDTO);
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
        public async Task<IActionResult> UpdatePlaylist(int id, PlaylistDTO playlistDTO)
        {
            if (id != playlistDTO.PlaylistId)
            {
                return BadRequest();
            }
            var playlistEntity = await _context.Playlists.FindAsync(id);
            if (playlistEntity == null)
            {
                return NotFound();
            }
            playlistEntity.PlaylistName = playlistDTO.PlaylistName;
            playlistEntity.PlaylistDescription = playlistDTO.PlaylistDescription;
            playlistEntity.CreatedAt = playlistDTO.CreatedAt;
            _context.Entry(playlistEntity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlaylistExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok(new { message = "Playlist updated successfully", playlist = playlistEntity });
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
        public async Task<IActionResult> DeletePlaylist(int id)
        {
            var playlistEntity = await _context.Playlists.FindAsync(id);
            if (playlistEntity == null)
            {
                return NotFound($"Playlist with ID {id} not found.");
            }

            _context.Playlists.Remove(playlistEntity);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Playlist deleted successfully", playlistId = id });
        }

        private bool PlaylistExists(int id)
        {
            return _context.Playlists.Any(p => p.PlaylistId == id);
        }
    }
}
