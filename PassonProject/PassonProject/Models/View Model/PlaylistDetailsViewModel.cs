using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using PassonProject.Models;

namespace PassonProject.ViewModels
{
    public class PlaylistDetailsViewModel
    {
        public PlaylistDTO Playlist { get; set; }
        public List<SongDTO> SongsInPlaylist { get; set; }
        public List<SelectListItem> AvailableSongs { get; set; }
    }
}
