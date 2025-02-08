using System.ComponentModel.DataAnnotations;
namespace PassonProjectMusicPlaylist.Models
{
    public class Song
    {
        [Key]
        public int SongId { get; set; }

        [Required]
        public string Title { get; set; }
        
        [Required]
        public string Artist { get; set; }

        [Required]
        public string Genre { get; set; }
        [Required]
        public DateTime ReleaseDate { get; set; }

        public ICollection<PlaylistXSong> PlaylistXSongs { get; set; }
    }
}
