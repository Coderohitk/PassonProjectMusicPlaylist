using Microsoft.AspNetCore.Mvc;
using PassonProject.Interfaces;
using PassonProject.Models;
using PassonProject.ViewModels; // Import the ViewModel namespace
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PassonProject.Data;
using Microsoft.AspNetCore.Authorization;

namespace PassonProject.Controllers
{
    public class PlaylistPageController : Controller
    {
        private readonly IPlaylistService _playlistService;
        private readonly ISongService _songService;
        private readonly IPlaylistXSongService _playlistXSongService;
        private readonly ApplicationDbContext _context;

        // Constructor to inject services
        public PlaylistPageController(IPlaylistService playlistService, ISongService songService, IPlaylistXSongService playlistXSongService, ApplicationDbContext context)  // Inject ApplicationDbContext here
        {
            _playlistService = playlistService;
            _songService = songService;
            _playlistXSongService = playlistXSongService;
            _context = context;  // Assign the context to the private field
        }

        // Index action to display the list of playlists
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var playlistDtos = await _playlistService.GetAllPlaylistsAsync();  // This will return IEnumerable<PlaylistDTO>
            return View(playlistDtos);  // View expects IEnumerable<PlaylistDTO>
        }

        // Details action to display a single playlist's details
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var playlist = await _playlistService.GetPlaylistByIdAsync(id);
            if (playlist == null)
            {
                return NotFound();
            }

            // Fetch all songs linked to this playlist
            var playlistSongMappings = await _playlistXSongService.GetSongsByPlaylistIdAsync(id);

            // Extract the actual songs from the mappings
            var playlistSongs = playlistSongMappings
                                .Select(ps => ps.Song) // Assuming PlaylistXSongDTO has a Song property
                                .ToList();

            // Get all songs to populate the dropdown
            var allSongs = await _songService.ListSongsAsync();

            var viewModel = new PlaylistDetailsViewModel
            {
                Playlist = playlist,
                SongsInPlaylist = playlistSongs, // Now correctly mapped to SongDTO
                AvailableSongs = allSongs.Select(s => new SelectListItem
                {
                    Value = s.SongId.ToString(),
                    Text = s.Title
                }).ToList()
            };

            return View(viewModel);
        }


        // POST: Add song to playlist
        [HttpPost("AddSongToPlaylist")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSongToPlaylist(int playlistId, int songId)
        {
            // Validate that playlistId and songId are greater than 0
            if (playlistId <= 0 || songId <= 0)
            {
                return BadRequest("Invalid playlist or song ID.");
            }

            // Check if the playlist and song exist in the database
            var playlistExists = await _context.Playlists.AnyAsync(p => p.PlaylistId == playlistId);
            var songExists = await _context.Songs.AnyAsync(s => s.SongId == songId);

            if (!playlistExists)
            {
                return BadRequest($"Playlist with ID {playlistId} does not exist.");
            }

            if (!songExists)
            {
                return BadRequest($"Song with ID {songId} does not exist.");
            }

            // Call the service to add the song to the playlist
            var result = await _playlistXSongService.AddSongToPlaylistAsync(playlistId, songId);

            if (result)
            {
                // Successfully added the song to the playlist, redirect to the playlist details
                return RedirectToAction("Details", new { id = playlistId });
            }
            else
            {
                // If adding the song failed (e.g., the song already exists in the playlist), return an error
                return View("Error");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveSongFromPlaylist(int playlistId, int songId)
        {
            var result = await _playlistXSongService.RemoveSongFromPlaylistAsync(playlistId, songId);

            if (!result)
            {
                // Return an error if the song wasn't found in the playlist
                return NotFound($"Song with ID {songId} not found in playlist with ID {playlistId}.");
            }

            // If successful, redirect back to the playlist details page
            return RedirectToAction("Details", new { id = playlistId });
        }



        // GET: Display the Add Playlist form
        [HttpGet]
        public IActionResult Add()
        {
            return View(new PlaylistDTO());  // Ensure an empty PlaylistDTO is passed to the view
        }

        // POST: Handle form submission for adding a new playlist
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(PlaylistDTO playlistDto)
        {
            if (ModelState.IsValid)
            {
                var createdPlaylist = await _playlistService.AddPlaylistAsync(playlistDto);  // Add the playlist
                return RedirectToAction(nameof(List));  // Redirect to the playlist list after adding
            }

            return View(playlistDto);  // Return to the form if validation fails
        }

        // GET: Display the Edit Playlist form for a specific playlist
        [HttpGet] // Ensure this is a GET request
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var playlistDto = await _playlistService.GetPlaylistByIdAsync(id);

            if (playlistDto == null)
            {
                return NotFound(); // Return 404 if playlist not found
            }

            return View(playlistDto); // Return the Edit view with the playlist details
        }

        // POST: PlaylistPage/Edit/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, PlaylistDTO playlistDto)
        {
            if (id != playlistDto.PlaylistId)
            {
                return NotFound(); // Return 404 if IDs don't match
            }

            if (ModelState.IsValid)
            {
                var updatedPlaylist = await _playlistService.UpdatePlaylistAsync(id, playlistDto);
                if (!updatedPlaylist)
                {
                    return NotFound(); // Return 404 if playlist not found
                }

                return RedirectToAction(nameof(List)); // Redirect to the playlist list after updating
            }

            return View(playlistDto); // Return to the form if validation fails
        }

        // GET: Display the confirmation form to delete a playlist
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var playlistDto = await _playlistService.GetPlaylistByIdAsync(id);  // Fetch the playlist by ID

            if (playlistDto == null)
            {
                return NotFound();  // Return 404 if playlist not found
            }

            return View(playlistDto);  // Return confirmation view with the playlist details
        }

        // POST: Handle the actual deletion of the playlist
        [HttpPost, ActionName("Delete")]  // ActionName to match the delete action
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deleted = await _playlistService.DeletePlaylistAsync(id);  // Delete the playlist

            if (!deleted)
            {
                return NotFound();  // Return 404 if playlist not found
            }

            return RedirectToAction(nameof(List));  // Redirect to the playlist list after deletion
        }
    }
}
