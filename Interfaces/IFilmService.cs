using LMDb.Models;

namespace LMDb.Interfaces
{
    public interface IFilmService
    {
        List<Film> GetAllFilms();
        Film? GetFilmById(int id);
        Film AddFilm(Film newFilm);
        bool UpdateFilm(Film updatedFilm);
        bool DeleteFilm(int id);
    }
}
