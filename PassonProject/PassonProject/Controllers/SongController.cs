using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PassonProject.Data;
using PassonProject.Models;

namespace PassonProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SongController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a list of all songs with their corresponding playlists and added date.
        /// </summary>
        /// <returns>A list of songs with associated playlists and added dates.</returns>
        /// <response code="200">Returns a list of songs with their playlist details.</response>
        /// <example>
        /// GET: api/Song/listSongs
        /// </example>
        [Route("listSongs")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SongDTO>>> ListSongs()
        {
            var songs = await _context.Songs
                .Include(s => s.PlaylistXSongs)
                .ThenInclude(ps => ps.Playlist)
                .ToListAsync();

            var songsDTOS = songs.Select(s => new SongDTO
            {
                SongId = s.SongId,
                Title = s.Title,
                Artist = s.Artist,
                Genre = s.Genre,
                ReleaseDate = s.ReleaseDate,
                PlaylistDetails = s.PlaylistXSongs.Select(ps => new PlaylistSongDetailDTO
                {
                    PlaylistName = ps.Playlist.PlaylistName,
                    AddedDate = ps.AddedDate
                }).ToList()
            }).ToList();

            return Ok(songsDTOS);
        }

        /// <summary>
        /// Retrieves a specific song by its ID, including its playlist details.
        /// </summary>
        /// <param name="id">The ID of the song to retrieve.</param>
        /// <returns>The details of the requested song.</returns>
        /// <response code="200">Returns the song and its playlist details.</response>
        /// <response code="404">If the song with the specified ID is not found.</response>
        /// <example>
        /// GET: api/Song/FindSong/1
        /// </example>
        [Route(template: "FindSong/{id}")]
        [HttpGet]
        public async Task<ActionResult<SongDTO>> FindSong(int id)
        {
            var song = await _context.Songs
                .Include(s => s.PlaylistXSongs)
                .ThenInclude(ps => ps.Playlist)
                .FirstOrDefaultAsync(s => s.SongId == id);

            if (song == null)
            {
                return NotFound();
            }

            var songDto = new SongDTO
            {
                SongId = song.SongId,
                Title = song.Title,
                Artist = song.Artist,
                Genre = song.Genre,
                ReleaseDate = song.ReleaseDate,
                PlaylistDetails = song.PlaylistXSongs.Select(ps => new PlaylistSongDetailDTO
                {
                    PlaylistName = ps.Playlist.PlaylistName,
                    AddedDate = ps.AddedDate
                }).ToList()
            };

            return Ok(songDto);
        }

        /// <summary>
        /// Adds a new song to the system.
        /// </summary>
        /// <param name="songDto">The details of the song to add.</param>
        /// <returns>The newly created song with its ID.</returns>
        /// <response code="201">Returns the created song details.</response>
        /// <response code="400">If the provided song data is invalid.</response>
        /// <example>
        /// POST: api/Song/AddSong
        /// Body:
        /// {
        ///   "title": "Song Title",
        ///   "artist": "Artist Name",
        ///   "genre": "Genre",
        ///   "releaseDate": "2025-02-08T18:35:08.126Z"
        /// }
        /// </example>
        [Route(template: "AddSong")]
        [HttpPost]
        public async Task<ActionResult<SongDTO>> AddSong(SongDTO songDto)
        {
            if (songDto == null)
            {
                return BadRequest("Invalid song data.");
            }

            var songEntity = new Song
            {
                Title = songDto.Title,
                Artist = songDto.Artist,
                Genre = songDto.Genre,
                ReleaseDate = songDto.ReleaseDate
            };

            _context.Songs.Add(songEntity);
            await _context.SaveChangesAsync();

            songDto.SongId = songEntity.SongId;

            return CreatedAtAction(nameof(FindSong), new { id = songEntity.SongId }, songDto);
        }

        /// <summary>
        /// Updates an existing song's details.
        /// </summary>
        /// <param name="id">The ID of the song to update.</param>
        /// <param name="songDto">The updated song details.</param>
        /// <returns>The updated song details.</returns>
        /// <response code="200">Returns the updated song details.</response>
        /// <response code="400">If the provided song data is invalid.</response>
        /// <response code="404">If the song with the specified ID is not found.</response>
        /// <example>
        /// PUT: api/Song/UpdateSong/1
        /// Body:
        /// {
        ///   "songId": 1,
        ///   "title": "Updated Song Title",
        ///   "artist": "Updated Artist Name",
        ///   "genre": "Updated Genre",
        ///   "releaseDate": "2025-02-08T18:35:08.126Z"
        /// }
        /// </example>
        [Route(template: "UpdateSong/{id}")]
        [HttpPut]
        public async Task<IActionResult> UpdateSong(int id, SongDTO songDto)
        {
            if (id != songDto.SongId)
            {
                return BadRequest("The ID in the URL does not match the SongId in the request body.");
            }

            var songEntity = await _context.Songs.FindAsync(id);
            if (songEntity == null)
            {
                return NotFound($"Song with ID {id} not found.");
            }

            songEntity.Title = songDto.Title;
            songEntity.Artist = songDto.Artist;
            songEntity.Genre = songDto.Genre;
            songEntity.ReleaseDate = songDto.ReleaseDate;

            _context.Entry(songEntity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SongExists(id))
                {
                    return NotFound($"Song with ID {id} no longer exists.");
                }
                else
                {
                    return StatusCode(500, "An error occurred while updating the song.");
                }
            }

            var updatedSong = new SongDTO
            {
                SongId = songEntity.SongId,
                Title = songEntity.Title,
                Artist = songEntity.Artist,
                Genre = songEntity.Genre,
                ReleaseDate = songEntity.ReleaseDate
            };

            return Ok(new { message = "Song updated successfully", song = updatedSong });
        }

        /// <summary>
        /// Deletes a song from the system by its ID.
        /// </summary>
        /// <param name="id">The ID of the song to delete.</param>
        /// <returns>A confirmation message after deletion.</returns>
        /// <response code="200">Returns a message confirming the song deletion.</response>
        /// <response code="404">If the song with the specified ID is not found.</response>
        /// <example>
        /// DELETE: api/Song/DeleteSong/1
        /// </example>
        [Route(template: "DeleteSong/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteSong(int id)
        {
            var songEntity = await _context.Songs.FindAsync(id);
            if (songEntity == null)
            {
                return NotFound();
            }

            _context.Songs.Remove(songEntity);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Song deleted successfully", songId = id });
        }

        private bool SongExists(int id)
        {
            return _context.Songs.Any(s => s.SongId == id);
        }
    }
}
