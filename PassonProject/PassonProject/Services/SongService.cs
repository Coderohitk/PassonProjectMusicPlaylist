using Microsoft.EntityFrameworkCore;
using PassonProject.Data;
using PassonProject.Interfaces;
using PassonProject.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PassonProject.Services
{
    public class SongService : ISongService
    {
        private readonly ApplicationDbContext _context;

        public SongService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SongDTO>> ListSongsAsync()
        {
            var songs = await _context.Songs.Include(s => s.PlaylistXSongs).ThenInclude(ps => ps.Playlist).ToListAsync();
            return songs.Select(s => new SongDTO
            {
                SongId = s.SongId,
                Title = s.Title,
                Artist = s.Artist,
                Genre = s.Genre,
                ReleaseDate = s.ReleaseDate,
                PlaylistDetails = s.PlaylistXSongs.Select(ps => new PlaylistSongDetailDTO
                {
                    PlaylistName = ps.Playlist.PlaylistName,
                    AddedDate = ps.AddedDate
                }).ToList()
            }).ToList();
        }

        public async Task<SongDTO> FindSongAsync(int id)
        {
            var song = await _context.Songs.Include(s => s.PlaylistXSongs).ThenInclude(ps => ps.Playlist).FirstOrDefaultAsync(s => s.SongId == id);
            if (song == null) return null;

            return new SongDTO
            {
                SongId = song.SongId,
                Title = song.Title,
                Artist = song.Artist,
                Genre = song.Genre,
                ReleaseDate = song.ReleaseDate,
                PlaylistDetails = song.PlaylistXSongs.Select(ps => new PlaylistSongDetailDTO
                {
                    PlaylistName = ps.Playlist.PlaylistName,
                    AddedDate = ps.AddedDate
                }).ToList()
            };
        }

        public async Task<SongDTO> AddSongAsync(SongDTO songDto)
        {
            var song = new Song
            {
                Title = songDto.Title,
                Artist = songDto.Artist,
                Genre = songDto.Genre,
                ReleaseDate = songDto.ReleaseDate
            };
            _context.Songs.Add(song);
            await _context.SaveChangesAsync();
            songDto.SongId = song.SongId;
            return songDto;
        }

        public async Task<SongDTO> UpdateSongAsync(int id, SongDTO songDto)
        {
            var song = await _context.Songs.FindAsync(id);
            if (song == null) return null;

            song.Title = songDto.Title;
            song.Artist = songDto.Artist;
            song.Genre = songDto.Genre;
            song.ReleaseDate = songDto.ReleaseDate;
            _context.Entry(song).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return songDto;
        }

        public async Task<bool> DeleteSongAsync(int id)
        {
            var song = await _context.Songs.FindAsync(id);
            if (song == null) 
             return false;

            _context.Songs.Remove(song);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}