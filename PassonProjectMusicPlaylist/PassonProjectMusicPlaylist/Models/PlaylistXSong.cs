using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PassonProjectMusicPlaylist.Models
{
    public class PlaylistXSong
    {
        [Key]
        public int PlaylistXSongId { get; set; }  // Primary Key (optional, but not needed for a bridge table)

        [ForeignKey(nameof(Playlist))]  // Matches Navigation Property
        public int PlaylistId { get; set; }
        public Playlist Playlist { get; set; }  // Navigation Property

        [ForeignKey(nameof(Song))]  // Matches Navigation Property
        public int SongId { get; set; }
        public Song Song { get; set; }
    }
}
