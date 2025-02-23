using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PassonProject.Models
{
    public class PlaylistXSong
    {
        [Key]
        public int PlaylistXSongId { get; set; }

        [ForeignKey("Playlist")]
        public int PlaylistId { get; set; }
        public Playlist Playlist { get; set; }

        [ForeignKey("Song")]
        public int SongId { get; set; }
        public Song Song { get; set; }

        public DateTime AddedDate { get; set; }
    }
   
        public class PlaylistXSongDTO
        {
            public int PlaylistId { get; set; }
            public int SongId { get; set; }
            public DateTime AddedDate { get; set; }
        public PlaylistDTO Playlist { get; set; }

        // Add the Song property directly in the DTO
        public SongDTO Song { get; set; } // This should be your SongDTO or a similar class
    }
    }

