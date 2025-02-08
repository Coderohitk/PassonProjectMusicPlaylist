using System.ComponentModel.DataAnnotations;
namespace PassonProject.Models
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
    public class SongDTO
    {
        public int SongId { get; set; }

        public string Title { get; set; }
        public string Artist { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Genre { get; set; }

        public List<PlaylistSongDetailDTO>? PlaylistDetails { get; set; }
    }
    public class PlaylistSongDetailDTO
    {
        public string PlaylistName { get; set; }
        public DateTime AddedDate { get; set; }
    }
}