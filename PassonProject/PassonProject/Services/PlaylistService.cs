using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PassonProject.Data;
using PassonProject.Interfaces;
using PassonProject.Models;

namespace PassonProject.Services
{
    public class PlaylistService : IPlaylistService
    {
        private readonly ApplicationDbContext _context;

        public PlaylistService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all playlists
        public async Task<IEnumerable<PlaylistDTO>> GetAllPlaylistsAsync()
        {
            var playlists = await _context.Playlists.ToListAsync();
            return playlists.Select(p => new PlaylistDTO
            {
                PlaylistId = p.PlaylistId,
                PlaylistName = p.PlaylistName,
                PlaylistDescription = p.PlaylistDescription,
                CreatedAt = p.CreatedAt
            }).ToList();
        }

        // Get a specific playlist by ID
        public async Task<PlaylistDTO> GetPlaylistByIdAsync(int id)
        {
            var playlistEntity = await _context.Playlists.FindAsync(id);
            if (playlistEntity == null)
            {
                return null;
            }

            return new PlaylistDTO
            {
                PlaylistId = playlistEntity.PlaylistId,
                PlaylistName = playlistEntity.PlaylistName,
                PlaylistDescription = playlistEntity.PlaylistDescription,
                CreatedAt = playlistEntity.CreatedAt
            };
        }

        // Add a new playlist
        public async Task<PlaylistDTO> AddPlaylistAsync(PlaylistDTO playlistDTO)
        {
            var playlistEntity = new Playlist
            {
                PlaylistName = playlistDTO.PlaylistName,
                PlaylistDescription = playlistDTO.PlaylistDescription,
                CreatedAt = playlistDTO.CreatedAt
            };

            _context.Playlists.Add(playlistEntity);
            await _context.SaveChangesAsync();
            playlistDTO.PlaylistId = playlistEntity.PlaylistId;
            return playlistDTO;
        }

        // Update an existing playlist
        public async Task<bool> UpdatePlaylistAsync(int id, PlaylistDTO playlistDTO)
        {
            var playlistEntity = await _context.Playlists.FindAsync(id);
            if (playlistEntity == null)
            {
                return false;
            }

            playlistEntity.PlaylistName = playlistDTO.PlaylistName;
            playlistEntity.PlaylistDescription = playlistDTO.PlaylistDescription;
            playlistEntity.CreatedAt = playlistDTO.CreatedAt;

            _context.Entry(playlistEntity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        // Delete a playlist
        public async Task<bool> DeletePlaylistAsync(int id)
        {
            var playlistEntity = await _context.Playlists.FindAsync(id);
            if (playlistEntity == null)
            {
                return false;
            }

            _context.Playlists.Remove(playlistEntity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
