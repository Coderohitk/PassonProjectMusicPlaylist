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
        /// 
        /// </summary>
        /// <returns></returns>
        // Get a list of songs with their corresponding playlists and added date
        [Route("listSongs")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SongDTO>>> ListSongs()
        {
            var songs = await _context.Songs
                .Include(s => s.PlaylistXSongs)  // Include the junction table
                .ThenInclude(ps => ps.Playlist) // Include the related Playlist
                .ToListAsync();

            var songsDTOS = songs.Select(s => new SongDTO
            {
                SongId = s.SongId,
                Title = s.Title,
                Artist = s.Artist,
                Genre = s.Genre,
                ReleaseDate = s.ReleaseDate,
                // Join the playlist names and added dates for each song
                PlaylistDetails = s.PlaylistXSongs.Select(ps => new PlaylistSongDetailDTO
                {
                    PlaylistName = ps.Playlist.PlaylistName,
                    AddedDate = ps.AddedDate
                }).ToList()
            }).ToList();

            return Ok(songsDTOS);
        }
        [Route(template: ("FindSong/{id}"))]
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

            songDto.SongId = songEntity.SongId; // Return the generated SongId

            return CreatedAtAction(nameof(FindSong), new { id = songEntity.SongId }, songDto);
        }
        [Route(template:("UpdateSong/{id}"))]
        [HttpPut]
        public async Task<IActionResult> UpdateSong(int id, SongDTO songDto)
        {
            if (id != songDto.SongId)
            {
                return BadRequest();
            }

            var songEntity = await _context.Songs.FindAsync(id);
            if (songEntity == null)
            {
                return NotFound();
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
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        [Route(template:("DeleteSong/{id}"))]
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

            return NoContent();
        }
        private bool SongExists(int id)
        {
            return _context.Songs.Any(s => s.SongId == id);
        }
    }
}
