using LMDb.Models;
using LMDb.Services;

namespace LMDb
{
    class Program
    {
        // Instantiate the FilmService - NOTE: This is where DI will be useful later!
        private static readonly FilmService _filmService = new FilmService(); // create instance of FilmService via direct instantiation [new FilmService()]

        static void Main(string[] args)
        {
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
            Console.WriteLine("\n---Film Menu ---");
            Console.WriteLine("1. List All Films");
            Console.WriteLine("2. View Film Details");
            Console.WriteLine("3. Add New Film");
            Console.WriteLine("4. Update Film");
            Console.WriteLine("5. Delete Film");
            Console.WriteLine("Q. Quit)");
            Console.Write("Select an option...");
        }

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
                Console.WriteLine(film); // Uses the ToString() override in Film.cs
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

            Console.WriteLine("Enter Year: ");
            // int validation
            newFilm.Year = int.Parse(Console.ReadLine()); // might break...

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

            Console.WriteLine($"Updating Film: {existingFilm.Title}");
            var updatedFilm = new Film { Id = id }; // new film obj with same ID as user entered

            Console.WriteLine($"Enter new Title (or leave blank to keep '{existingFilm.Title})");
            string? newTitle = Console.ReadLine();
            updatedFilm.Title = string.IsNullOrWhiteSpace(newTitle) ? existingFilm.Title : newTitle;

            Console.WriteLine($"Enter new Director (or leave blank to keep '{existingFilm.Director})");
            string? newDirector = Console.ReadLine();
            updatedFilm.Director = string.IsNullOrWhiteSpace(newDirector) ? existingFilm.Director : newDirector;

            Console.WriteLine($"Eneter new Year (or leave blank to keep '{existingFilm.Year}')");
            string? yearInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(yearInput))
            {
                updatedFilm.Year = existingFilm.Year; // user left blank; keep existing year
            }
            else
            {
                while (!int.TryParse(yearInput, out int year) || year < 1888 || year > DateTime.Now.Year + 5)
                {
                    Console.WriteLine($"Invalid year. Palese enter a valid year (e.g., between 1888 and {{DateTime.Now.Year + 5}}) or leave blank to keep current value.");
                    Console.Write("Enter new Year: ");
                    yearInput = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(yearInput)) // allow user to exit loop by leaving blank
                    {
                        updatedFilm.Year = existingFilm.Year;
                        break; // exit while loop
                    }
                }
                if (int.TryParse(yearInput, out int parsedYear))
                {
                    updatedFilm.Year = parsedYear; // assign only if successfully parsed
                }
            }
            if(_filmService.UpdateFilm(updatedFilm))
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
}