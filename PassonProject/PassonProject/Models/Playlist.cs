using System.ComponentModel.DataAnnotations;
namespace PassonProject.Models
{
    public class Playlist
    {
        [Key]
        public int PlaylistId { get; set; }  
        public string Name { get; set; } 
        public string Description { get; set; }

    }
}
