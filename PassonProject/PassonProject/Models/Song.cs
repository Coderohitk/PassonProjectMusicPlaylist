using System.ComponentModel.DataAnnotations;
namespace PassonProject.Models
{
    public class Song
    {
        [Key]
        public int SongId { get; set; }  // Primary Key
        public string Title { get; set; } 
        public string Artist { get; set; } 
        public string Genre { get; set; }

        // Navigation Property - Many-to-Many with Playlists

    }
}
