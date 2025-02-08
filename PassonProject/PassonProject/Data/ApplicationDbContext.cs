using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PassonProject.Models;

namespace PassonProject.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<Song> Songs { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<PassonProject.Models.PlaylistXSong> PlaylistXSong { get; set; } = default!;
    }
}
