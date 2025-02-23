using Microsoft.AspNetCore.Mvc;
using PassonProject.Services;
using PassonProject.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using PassonProject.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace PassonProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongController : ControllerBase
    {
        private readonly ISongService _songService;

        public SongController(ISongService songService)
        {
            _songService = songService;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("listSongs")]
        public async Task<ActionResult<IEnumerable<SongDTO>>> ListSongs()
        {
            var songs = await _songService.ListSongsAsync();
            return Ok(songs);
        }

        [HttpGet("FindSong/{id}")]
        public async Task<ActionResult<SongDTO>> FindSong(int id)
        {
            var song = await _songService.FindSongAsync(id);
            if (song == null)
            {
                return NotFound();
            }
            return Ok(song);
        }

        [HttpPost("AddSong")]
        public async Task<ActionResult<SongDTO>> AddSong(SongDTO songDto)
        {
            var createdSong = await _songService.AddSongAsync(songDto);
            if (createdSong == null)
            {
                return BadRequest("Invalid song data.");
            }
            return CreatedAtAction(nameof(FindSong), new { id = createdSong.SongId }, createdSong);
        }

        [HttpPut("UpdateSong/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateSong(int id, SongDTO songDto)
        {
            var updatedSong = await _songService.UpdateSongAsync(id, songDto);
            if (updatedSong == null)
            {
                return NotFound($"Song with ID {id} not found.");
            }
            return Ok(new { message = "Song updated successfully", song = updatedSong });
        }

        [HttpDelete("DeleteSong/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteSong(int id)
        {
            var deleted = await _songService.DeleteSongAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return Ok(new { message = "Song deleted successfully", songId = id });
        }
    }
}
