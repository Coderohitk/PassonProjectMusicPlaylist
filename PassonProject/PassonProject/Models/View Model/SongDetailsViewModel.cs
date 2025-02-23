using Microsoft.AspNetCore.Mvc.Rendering;
using PassonProject.Models;

namespace PassonProject.ViewModels
{
    public class SongDetailsViewModel
    {
        public SongDTO Song { get; set; } // The song details
        public List<PlaylistDTO> PlaylistsForSong { get; set; } // The playlists the song belongs to
        public List<SelectListItem> AvailablePlaylists { get; set; } // List of available playlists for adding
    }
}
