using LMDb.Models;

namespace LMDb.Services
{
    public class FilmService
    {
        private readonly string _filePath = Path.Combine("Data", "films.json"); // Path relative to execution dir
        private List<Film> _films;
        private int _nextId = 1;

        public FilmService()
        {
            _films = LoadFilmsFromFile();
            // Determine the next available ID based on loaded films
            if (_films.Any())
            {
                _nextId = _films.Max(f => f.Id) + 1;
            }
        }

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
            if(filmToRemove == null)
            {
                return false;
            }

            _films.Remove(filmToRemove);
            SaveChangesToFile();
            return true;
        }

        // --- Helper Methods for File I/O ---


    }
}
