# Passon Project API

This API manages playlists and songs within the Passon project. It provides endpoints for handling CRUD operations on both songs and playlists, with relationships between songs and playlists managed through a many-to-many relationship using a junction table `PlaylistXSong`.

## **Endpoints**

### **Playlist Endpoints**
1. **ListPlaylists**  
   `GET /api/playlists/ListPlaylists`  
   Retrieves a list of all playlists with their details.

2. **FindPlaylist**  
   `GET /api/playlists/FindPlaylist/{id}`  
   Retrieves a specific playlist by its ID.

3. **AddPlaylist**  
   `POST /api/playlists/AddPlaylist`  
   Creates a new playlist.

4. **UpdatePlaylist**  
   `PUT /api/playlists/UpdatePlaylist/{id}`  
   Updates an existing playlist by its ID.

5. **DeletePlaylist**  
   `DELETE /api/playlists/DeletePlaylist/{id}`  
   Deletes a playlist by its ID.

6. **ListSongsForPlaylist**  
   `GET /api/playlists/ListSongsForPlaylist/{playlistId}`  
   Retrieves a list of songs for a specific playlist, including song details like title, artist, release date, and added date to the playlist.

### **Song Endpoints**
1. **ListSongs**  
   `GET /api/songs/listSongs`  
   Retrieves a list of all songs with their associated playlists and the date added to each playlist.

2. **FindSong**  
   `GET /api/songs/FindSong/{id}`  
   Retrieves a specific song by its ID, along with its associated playlists and added dates.

3. **AddSong**  
   `POST /api/songs/AddSong`  
   Creates a new song.

4. **UpdateSong**  
   `PUT /api/songs/UpdateSong/{id}`  
   Updates an existing song by its ID.

5. **DeleteSong**  
   `DELETE /api/songs/DeleteSong/{id}`  
   Deletes a song by its ID.

## **Database Structure**
- **Playlists**  
   Playlists are collections of songs. Each playlist has a name, description, and creation date.

- **Songs**  
   Each song has a title, artist, genre, and release date.

- **PlaylistXSong**  
   This is the junction table that connects playlists and songs, representing the many-to-many relationship between them. It also stores the date when the song was added to the playlist.

## **DTOs (Data Transfer Objects)**
1. **PlaylistDTO**  
   Represents a playlist, including its ID, name, description, and creation date.

2. **SongDTO**  
   Represents a song, including its ID, title, artist, genre, release date, and playlist details (name and added date).

3. **PlaylistSongDetailDTO**  
   Represents the details of a song in a playlist, including the playlist name and the date the song was added.

## **Error Handling**
- **Bad Request (400)**: If the request data is invalid, such as missing fields or mismatched IDs.
- **Not Found (404)**: If the requested resource (playlist or song) doesn't exist.
- **No Content (204)**: When a resource is successfully deleted or updated without returning any content.

## **Technology Stack**
- **Backend**: ASP.NET Core Web API
- **Database**: Entity Framework Core (EF Core) with a SQL Server or other relational database
- **Language**: C#

## **Setup and Configuration**
1. Clone the repository.
2. Set up the database connection in the `appsettings.json` file.
3. Run database migrations to ensure the necessary tables are created.
4. Start the application using `dotnet run`.

## **Contributing**
Feel free to fork the project, submit pull requests, or open issues if you encounter any problems or have suggestions for improvements.

