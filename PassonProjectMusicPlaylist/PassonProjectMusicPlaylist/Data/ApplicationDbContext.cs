using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PassonProjectMusicPlaylist.Models;
namespace PassonProjectMusicPlaylist.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Song> Songs { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
