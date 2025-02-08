using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PassonProject.Data;
using PassonProject.Models;

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
        /// 
        /// </summary>
        /// <returns></returns>
        [Route(template: ("ListPlaylists"))]
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
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route(template: ("FindSong/{id}"))]
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
        [Route(template:("AddPlaylist"))]
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
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="playlistDTO"></param>
        /// <returns></returns>
        [Route(template:("UpdatePlaylist/{id}"))]
        [HttpPut]
        public async Task<ActionResult> UpdatePlaylist(int id, PlaylistDTO playlistDTO)
        {
            if(id != playlistDTO.PlaylistId)
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
            return NoContent();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route(template:"DeletePlaylist/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeletePlaylist(int id)
        {
            var playlistEntity = await _context.Playlists.FindAsync(id);
            if (playlistEntity == null)
            {
                return NotFound();
            }

            _context.Playlists.Remove(playlistEntity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PlaylistExists(int id)
        {
            return _context.Playlists.Any(p => p.PlaylistId == id);
        }
        [Route("ListSongsForPlaylist/{playlistId}")]
        [HttpGet]
        public async Task<IActionResult> ListSongsForPlaylist(int playlistId)
        {
            var playlist = await _context.PlaylistXSong
                .Where(ps => ps.PlaylistId == playlistId)
                .Include(ps => ps.Song) // Ensure the Song is included
                .Include(ps => ps.Playlist) // Include the Playlist information
                .ToListAsync();

            if (!playlist.Any())
            {
                return NotFound($"No songs found for playlist with ID {playlistId}.");
            }

            var songs = playlist.Select(ps => new SongDTO
            {
                SongId = ps.Song.SongId,
                Title = ps.Song.Title,
                Artist = ps.Song.Artist,
                ReleaseDate = ps.Song.ReleaseDate,
                Genre = ps.Song.Genre,
                PlaylistDetails = ps.Song.PlaylistXSongs
                    .Where(x => x.PlaylistId == playlistId)
                    .Select(x => new PlaylistSongDetailDTO
                    {
                        PlaylistName = x.Playlist.PlaylistName,
                        AddedDate = x.AddedDate
                    }).ToList()
            }).ToList();

            return Ok(songs);
        }

    }
}
