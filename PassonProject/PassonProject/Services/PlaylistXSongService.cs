using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PassonProject.Interfaces;
using PassonProject.Models;
using PassonProject.Data;

namespace PassonProject.Services
{
    public class PlaylistXSongService : IPlaylistXSongService
    {
        private readonly ApplicationDbContext _context;

        // Constructor that accepts the ApplicationDbContext
        public PlaylistXSongService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all songs in a specific playlist by playlistId
        public async Task<IEnumerable<PlaylistXSongDTO>> GetSongsByPlaylistIdAsync(int playlistId)
        {
            return await _context.PlaylistXSong
                .Where(ps => ps.PlaylistId == playlistId)
                .Include(ps => ps.Song) // Ensure Song is included in the query
                .Select(ps => new PlaylistXSongDTO
                {
                    PlaylistId = ps.PlaylistId,
                    SongId = ps.SongId,
                    AddedDate = ps.AddedDate,
                    Song = new SongDTO
                    {
                        SongId = ps.Song.SongId,
                        Title = ps.Song.Title
                    }
                })
                .ToListAsync();
        }

        // Get all playlists that contain a specific song by songId
        public async Task<IEnumerable<PlaylistXSongDTO>> GetPlaylistsBySongIdAsync(int songId)
        {
            return await _context.PlaylistXSong
                .Where(ps => ps.SongId == songId)
                .Include(ps => ps.Playlist)  // Ensure that the Playlist is included
                .Select(ps => new PlaylistXSongDTO
                {
                    PlaylistId = ps.PlaylistId,
                    SongId = ps.SongId,
                    AddedDate = ps.AddedDate,
                    Playlist = new PlaylistDTO  // Add the full Playlist information
                    {
                        PlaylistId = ps.Playlist.PlaylistId,
                        PlaylistName = ps.Playlist.PlaylistName,  // Add any other properties you need from PlaylistDTO
                        PlaylistDescription = ps.Playlist.PlaylistDescription // Example: if you want the description too
                    }
                })
                .ToListAsync();
        }
        public async Task<bool> AddPlaylistToSongAsync(int songId, int playlistId)
        {
            // Check if the playlist is already associated with the song
            var existingEntry = await _context.PlaylistXSong
                .FirstOrDefaultAsync(ps => ps.PlaylistId == playlistId && ps.SongId == songId);

            if (existingEntry != null)
            {
                return false; // Playlist is already associated with the song
            }

            // Create a new PlaylistXSong entry
            var newEntry = new PlaylistXSong
            {
                PlaylistId = playlistId,
                SongId = songId,
                AddedDate = DateTime.UtcNow // Set the current date for when the playlist is added to the song
            };

            // Add the new entry to the database
            _context.PlaylistXSong.Add(newEntry);
            var result = await _context.SaveChangesAsync();

            return result > 0; // Return true if the playlist was successfully added to the song
        }
        public async Task<bool> RemovePlaylistFromSongAsync(int songId, int playlistId)
        {
            // Find the PlaylistXSong entry to remove
            var playlistXSongEntity = await _context.PlaylistXSong
                .FirstOrDefaultAsync(ps => ps.PlaylistId == playlistId && ps.SongId == songId);

            if (playlistXSongEntity == null)
            {
                return false; // Return false if the playlist is not associated with the song
            }

            // Remove the entry from the PlaylistXSong table
            _context.PlaylistXSong.Remove(playlistXSongEntity);
            var result = await _context.SaveChangesAsync();

            return result > 0; // Return true if the playlist was successfully removed from the song
        }

        // Add a song to a playlist
        public async Task<bool> AddSongToPlaylistAsync(int playlistId, int songId)
        {
            // Check if the song is already in the playlist
            var existingEntry = await _context.PlaylistXSong
                .FirstOrDefaultAsync(ps => ps.PlaylistId == playlistId && ps.SongId == songId);

            if (existingEntry != null)
            {
                return false; // Song already exists in the playlist
            }

            // Create a new PlaylistXSong entry
            var newEntry = new PlaylistXSong
            {
                PlaylistId = playlistId,
                SongId = songId,
                AddedDate = DateTime.UtcNow // Set the current date for when the song is added
            };

            // Add the new entry to the database
            _context.PlaylistXSong.Add(newEntry);
            var result = await _context.SaveChangesAsync();

            return result > 0; // Return true if the song was successfully added
        }


        // Remove a song from a playlist
        public async Task<bool> RemoveSongFromPlaylistAsync(int playlistId, int songId)
        {
            var playlistXSongEntity = await _context.PlaylistXSong
                .FirstOrDefaultAsync(ps => ps.PlaylistId == playlistId && ps.SongId == songId);

            if (playlistXSongEntity == null)
            {
                return false; // Return false if the song is not found in the playlist
            }

            // Remove the song from the playlist
            _context.PlaylistXSong.Remove(playlistXSongEntity);
            var result = await _context.SaveChangesAsync();

            return result > 0; // Return true if the song was successfully removed
        }
    }
}
