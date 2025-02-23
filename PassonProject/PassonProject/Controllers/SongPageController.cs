using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PassonProject.Interfaces;  // Correct namespace for ISongService
using PassonProject.Models;     // Correct namespace for SongDTO
using PassonProject.Services;
using PassonProject.ViewModels;
using System.Threading.Tasks;

namespace PassonProject.Controllers
{
    public class SongPageController : Controller
    {
        private readonly ISongService _songService;
        private readonly IPlaylistService _playlistService;
        private readonly IPlaylistXSongService _playlistXSongService;

        // Inject ISongService through constructor
        public SongPageController(ISongService songService,IPlaylistXSongService playlistXSongService,IPlaylistService playlistService)
        {
            _songService = songService;
            _playlistXSongService = playlistXSongService;
            _playlistService = playlistService;

        }

        // Index action to display the list of songs
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var songDtos = await _songService.ListSongsAsync();  // This will return IEnumerable<SongDTO>
            return View(songDtos);  // View expects IEnumerable<SongDTO>
        }

        // Details action to display a single song's details
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var songDto = await _songService.FindSongAsync(id);  // Get the song by ID
            if (songDto == null)
            {
                return NotFound();  // Return 404 if song not found
            }

            // Fetch all playlists that contain this song
            var playlistMappings = await _playlistXSongService.GetPlaylistsBySongIdAsync(id);

            // Extract the playlists the song belongs to
            var playlistsForSong = playlistMappings
                .Select(ps => ps.Playlist) // This will give you the actual PlaylistDTO
                .ToList();

            // Get all available playlists for adding this song
            var allPlaylists = await _playlistService.GetAllPlaylistsAsync();

            // Filter available playlists where the song is not already added
            var availablePlaylists = allPlaylists
                .Where(p => !playlistsForSong.Any(existingPlaylist => existingPlaylist.PlaylistId == p.PlaylistId)) // Check that the playlist is not in the song's playlists
                .Select(p => new SelectListItem
                {
                    Value = p.PlaylistId.ToString(),
                    Text = p.PlaylistName
                })
                .ToList();

            // Create the view model to pass to the view
            var songDetailsViewModel = new SongDetailsViewModel
            {
                Song = songDto,
                PlaylistsForSong = playlistsForSong,  // All playlists this song belongs to
                AvailablePlaylists = availablePlaylists  // Available playlists for adding this song
            };

            // Return the view with the view model
            return View(songDetailsViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPlaylistToSong(int songId, int playlistId)
        {
            // Call the service to add the playlist to the song
            var result = await _playlistXSongService.AddPlaylistToSongAsync(songId, playlistId);

            if (!result)
            {
                // Handle failure (e.g., playlist already associated with the song)
                return View("Error");
            }

            // Redirect back to the song details page
            return RedirectToAction("Details", new { id = songId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemovePlaylistFromSong(int songId, int playlistId)
        {
            // Call the service to remove the playlist from the song
            var result = await _playlistXSongService.RemovePlaylistFromSongAsync(songId, playlistId);

            if (!result)
            {
                // Handle failure (e.g., no such playlist associated with the song)
                return View("Error");
            }

            // Redirect back to the song details page
            return RedirectToAction("Details", new { id = songId });
        }





        // GET: Display the Add Song form
        [HttpGet]
        public IActionResult Add()
        {
            return View(new SongDTO());  // Ensure an empty SongDTO is passed to the view
        }

        // POST: Handle form submission for adding a new song
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(SongDTO songDto)
        {
            if (ModelState.IsValid)
            {
                var createdSong = await _songService.AddSongAsync(songDto);  // Add the song
                return RedirectToAction(nameof(List));  // Redirect to the song list after adding
            }

            return View(songDto);  // Return to the form if validation fails
        }

        // GET: Display the Edit Song form for a specific song
        [HttpGet]// Ensure this is a GET request
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var songDto = await _songService.FindSongAsync(id);

            if (songDto == null)
            {
                return NotFound(); // Return 404 if song not found
            }

            return View(songDto); // Return the Edit view with the song details
        }

        // POST: SongPage/Edit/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, SongDTO songDto)
        {
            if (id != songDto.SongId)
            {
                return NotFound(); // Return 404 if IDs don't match
            }

            if (ModelState.IsValid)
            {
                var updatedSong = await _songService.UpdateSongAsync(id, songDto);
                if (updatedSong == null)
                {
                    return NotFound(); // Return 404 if song not found
                }

                return RedirectToAction(nameof(List)); // Redirect to the song list after updating
            }

            return View(songDto); // Return to the form if validation fails
        }

        // GET: Display the confirmation form to delete a song
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var songDto = await _songService.FindSongAsync(id);  // Fetch the song by ID

            if (songDto == null)
            {
                return NotFound();  // Return 404 if song not found
            }

            return View(songDto);  // Return confirmation view with the song details
        }

        // POST: Handle the actual deletion of the song
        [HttpPost, ActionName("Delete")]  // ActionName to match the delete action
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deleted = await _songService.DeleteSongAsync(id);  // Delete the song

            if (!deleted)
            {
                return NotFound();  // Return 404 if song not found
            }

            return RedirectToAction(nameof(List));  // Redirect to the song list after deletion
        }
    }
}
