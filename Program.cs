using LMDb.Models;
using LMDb.Interfaces;
using LMDb.Services;

namespace LMDb
{
    class Program
    {
        //private static readonly FilmService _filmService = new FilmService(); // create instance of FilmService via direct instantiation [new FilmService()]
        private static IFilmService _filmService;

        static void Main(string[] args)
        {
            _filmService = new FilmService();

            Console.WriteLine("LocalMovieDatabase (LMDb), where films float in the ether");

            bool running = true;
            while (running)
            {
                ShowMenu();
                string? choice = Console.ReadLine();

                switch (choice?.ToLower())
                {
                    case "1":
                        ListAllFilms();
                        break;
                    case "2":
                        ViewFilmDetails();
                        break;
                    case "3":
                        AddNewFilm();
                        break;
                    case "4":
                        UpdateExistingFilm();
                        break;
                    case "5":
                        DeleteFilm();
                        break;
                    case "q":
                        running = false;
                        Console.WriteLine("Closing LMDb, goodbye");
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Choose another option");
                        break;
                }

                Console.WriteLine("\nPress Enter to continue...");
                Console.ReadLine();
                Console.Clear();
            }
        }

        static void ShowMenu()
        {
            Console.WriteLine("\n--- LMDb Film Menu ---");
            Console.WriteLine("1. List All Films");
            Console.WriteLine("2. View Film Details");
            Console.WriteLine("3. Add New Film");
            Console.WriteLine("4. Update Film");
            Console.WriteLine("5. Delete Film");
            Console.WriteLine("Q. Quit");
            Console.Write("Select an option...");
        }

        #region Helper methods
        static void ListAllFilms()
        {
            Console.WriteLine("\n--- All Films ---");
            var films = _filmService.GetAllFilms();
            if (!films.Any())
            {
                Console.WriteLine("No films found.");
                return;
            }
            foreach (var film in films)
            {
                Console.WriteLine(film); // uses the ToString() override in Film.cs
            }
        }

        static void ViewFilmDetails()
        {
            Console.WriteLine("Enter ID of the film to view: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var film = _filmService.GetFilmById(id);
                if (film != null)
                {
                    Console.WriteLine("\n--- Film Details ---");
                    Console.WriteLine($"ID: {film.Id}");
                    Console.WriteLine($"Title: {film.Title}");
                    Console.WriteLine($"Director: {film.Director}");
                    Console.WriteLine($"Year: {film.Year}");
                }
                else
                {
                    Console.WriteLine($"Film with ID {id} not found.");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format. Provide an int value");
            }
        }

        static void AddNewFilm()
        {
            Console.WriteLine("\n--- Add New Film ---");
            var newFilm = new Film(); // new up an instance of that bad boy class 💯

            Console.WriteLine("Enter Title: ");
            newFilm.Title = Console.ReadLine();

            Console.WriteLine("Enter Director: ");
            newFilm.Director = Console.ReadLine();

            Console.Write("Enter Year: ");
            int yearValue;
            // loop until valid integer year is entered within the specified range.
            while (true)
            {
                string? input = Console.ReadLine();
                if (int.TryParse(input, out yearValue) && yearValue >= 1888 && yearValue <= DateTime.Now.Year + 5)
                {
                    break; // valid input, exit loop
                }
                else
                {
                    Console.WriteLine($"Invalid year. Please enter a valid year (e.g., between 1888 and {DateTime.Now.Year + 5}).");
                    Console.Write("Enter Year: ");
                }
            }
            newFilm.Year = yearValue; // Assign the validated year.

            var addedFilm = _filmService.AddFilm(newFilm);
            Console.WriteLine($"Film added successfully with ID: {addedFilm.Id}");
        }

        static void UpdateExistingFilm()
        {
            Console.WriteLine("\n--- Update Film ---");
            Console.WriteLine("Enter ID of film to be updated: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID format.");
                return;
            }

            var existingFilm = _filmService.GetFilmById(id);
            if (existingFilm == null)
            {
                Console.WriteLine($"Film with ID {id} not found.");
                return;
            }

            Console.WriteLine($"Updating Film: ID {existingFilm.Id}, Title: {existingFilm.Title}");
            var updatedFilm = new Film
            {
                Id = existingFilm.Id, // keep the original ID
                Title = existingFilm.Title, // default to existing values
                Director = existingFilm.Director,
                Year = existingFilm.Year
            };

            Console.WriteLine($"Enter new Title (or leave blank to keep '{existingFilm.Title}')");
            string? newTitle = Console.ReadLine();
            updatedFilm.Title = string.IsNullOrWhiteSpace(newTitle) ? existingFilm.Title : newTitle;

            Console.WriteLine($"Enter new Director (or leave blank to keep '{existingFilm.Director}')");
            string? newDirector = Console.ReadLine();
            updatedFilm.Director = string.IsNullOrWhiteSpace(newDirector) ? existingFilm.Director : newDirector;

            Console.Write($"Enter new Year (current: '{existingFilm.Year}', leave blank to keep): ");
            string? yearInputStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(yearInputStr))
            {
                int newYearValue;
                while (true)
                {
                    if (int.TryParse(yearInputStr, out newYearValue) && newYearValue >= 1888 && newYearValue <= DateTime.Now.Year + 5)
                    {
                        updatedFilm.Year = newYearValue;
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"Invalid year. Please enter a valid year (e.g., between 1888 and {DateTime.Now.Year + 5}) or leave blank.");
                        Console.Write("Enter new Year: ");
                        yearInputStr = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(yearInputStr)) // allow to skip update here
                        {
                            break; // keep original year if blank is entered in the re-prompt
                        }
                    }
                }
            }
            // else, updatedFilmData.Year retains existingFilm.Year

            if (_filmService.UpdateFilm(updatedFilm))
            {
                Console.WriteLine("Film updated successfully.");
            }
            else
            {
                Console.WriteLine("Failed to update film.");
            }
        }

        static void DeleteFilm()
        {
            Console.Write("\n--- Delete Film ---");
            Console.WriteLine("Enter the ID of the film to delete: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var film = _filmService.GetFilmById(id);
                if (film != null)
                {
                    Console.WriteLine($"Are you sure you want to delete '{film.Title}' (ID: {id})? (y/n)");
                    string? confirm = Console.ReadLine();
                    if (confirm?.ToLower() == "y")
                    {
                        if (_filmService.DeleteFilm(id))
                        {
                            Console.WriteLine("Film deleted successfully");
                        }
                        else
                        {
                            Console.WriteLine($"Error: Failed to delete film with ID {id}. It might have been removed already...");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Deletion cancelled.");
                    }
                }
                else
                {
                    Console.WriteLine($"Film with ID {id} not found.");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format");
            }
        }
    }
    #endregion
}