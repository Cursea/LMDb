using LMDb.Interfaces;
using LMDb.Models;
using System.Text.Json;

namespace LMDb.Services
{
    public class FilmService : IFilmService
    {
        private readonly string _filePath = Path.Combine("Data", "films.json"); // Path relative to execution dir
        private List<Film> _films; // holds films in memory while app runs (\LMDb\bin\Debug\net8.0\Data)
        private int _nextId = 1; // tracks next available id for new films

        // CONSTRUCTOR
        // loads films from file where service is created; determines starting ids for new films
        public FilmService()
        {
            _films = LoadFilmsFromFile();
            // Determine the next available ID based on loaded films
            if (_films.Any())
            {
                _nextId = _films.Max(f => f.Id) + 1;
            }
        }

        #region --- CRUDs ---
        // READ ALL
        public List<Film> GetAllFilms()
        {
            return _films;
        }

        // READ One by ID
        public Film? GetFilmById(int id)
        {
            return _films.FirstOrDefault(f => f.Id == id);
        }

        // CREATE
        public Film AddFilm(Film newFilm)
        {
            newFilm.Id = _nextId++; // Assign next ID and increment
            _films.Add(newFilm);
            SaveChangesToFile();
            return newFilm;
        }

        // UPDATE
        public bool UpdateFilm(Film updatedFilm)
        {
            var existingFilm = _films.FirstOrDefault(f => f.Id == updatedFilm.Id);
            if (existingFilm == null)
            {
                return false; // Film not found
            }

            // update properties
            existingFilm.Title = updatedFilm.Title;
            existingFilm.Director = updatedFilm.Director;
            existingFilm.Year = updatedFilm.Year;

            SaveChangesToFile();
            return true;
        }

        // DELETE
        public bool DeleteFilm(int id)
        {
            var filmToRemove = _films.FirstOrDefault(f => f.Id == id);
            if (filmToRemove == null)
            {
                return false;
            }

            _films.Remove(filmToRemove);
            SaveChangesToFile();
            return true;
        }
        #endregion

        #region --- Helper Methods for File I/O ---

        private List<Film> LoadFilmsFromFile()
        {
            if (!File.Exists(_filePath))
            {
                // If the file doesn't exist, return an empty list
                // It will be created on the first save.
                return new List<Film>();
            }

            try
            {
                string json = File.ReadAllText(_filePath);
                // Handle empty or invalid JSON file
                if (string.IsNullOrWhiteSpace(json))
                {
                    return new List<Film>();
                }
                // Ensure deserialisation handles potential nulls
                var films = JsonSerializer.Deserialize<List<Film>>(json);
                return films ?? new List<Film>(); // return empty list if deserialisation results in null
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error accessing file at {_filePath}: {ex.Message}");
                return new List<Film>();
            }
        }

        private void SaveChangesToFile()
        {
            try
            {
                // ensure directory exists
                var directory = Path.GetDirectoryName(_filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // serialize with formatting for readability
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(_films, options);
                File.WriteAllText(_filePath, json);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error saving data to {_filePath}: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Access denied, PUNK. Cannot write to {_filePath}: {ex.Message}");
            }
        }
        #endregion

    }
}
