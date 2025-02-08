using System.ComponentModel.DataAnnotations;
namespace PassonProject.Models
{
    public class Playlist
    {
        [Key]
        public int PlaylistId { get; set; }
        [Required]
        public string PlaylistName { get; set; }
        [Required]
        public string PlaylistDescription { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }

        public ICollection<PlaylistXSong> PlaylistXSongs { get; set; }


    }
    public class PlaylistDTO
    {
        public int PlaylistId { get; set; }
        public string PlaylistName { get; set; }
        public string PlaylistDescription { get; set; }
        public DateTime CreatedAt { get; set; }
    }
   
}
