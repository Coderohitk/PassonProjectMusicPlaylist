using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PassonProject.Models
{
    public class PlaylistSong
    {
        [Key]
        public int PlaylistSongId { get; set; }
        [ForeignKey("Playlists")]
        public int PlaylistId { get; set; }  // Foreign Key from Playlist
        [ForeignKey("Songs")]
        public int SongId { get; set; }  // Foreign Key from Song

        // Navigation Properties

        public Playlist Playlist { get; set; }

        public Song Song { get; set; }
    }
}
