using GameRentalApp.Models;
using GameRentalApp.Services;
using System.Reflection;

var manager = new RentalManager();
manager.LoadData();

while (true)
{
    Console.Clear();
    Console.WriteLine("SYSTEM WYPOŻYCZALNI GIER");
    Console.WriteLine("1. Katalog Gier | 2. Wypożycz | 3. Nowy Klient | 4. Sprawdź Klienta | 5. ZWROT | 6. Lista Klientów | 7. Dodaj nową grę | 0. Wyjdź");
    Console.Write("\nWybór: ");
    var opt = Console.ReadLine();

    switch (opt)
    {
        case "1":
            {
                Console.WriteLine("\nLISTA GIER");
                if (!manager.Games.Any()) Console.WriteLine("Baza gier jest pusta.");
                foreach (var g in manager.Games)
                    Console.WriteLine($"ID: {g.Id} | {g.Title.PadRight(15)} | Status: {(g.IsAvailable ? "WOLNA" : "ZAJĘTA")} | Typ: {g.GetType().Name}");
                break;
            }
        case "2":
            {
                int gid = ReadInt("Podaj ID gry: ");
                int cid = ReadInt("Podaj ID klienta: ");
                int days = ReadInt("Na ile dni? ");
                Console.WriteLine($"\n{manager.RentGame(gid, cid, days)}");
                break;
            }

        case "3":
            {
                Console.Write("Imię i nazwisko: "); string name = Console.ReadLine() ?? "";
                Console.Write("Email: "); string email = Console.ReadLine() ?? "";
                if (string.IsNullOrWhiteSpace(name)) Console.WriteLine("Błąd: Imię nie może być puste!");
                else
                {
                    manager.AddCustomer(new Customer { FullName = name, Email = email });
                    Console.WriteLine("Klient dodany pomyślnie.");
                }
                break;
            }

        case "4":
            {
                int searchCid = ReadInt("Podaj ID klienta: ");
                if (!manager.Customers.Any(c => c.Id == searchCid)) Console.WriteLine("Błąd: Taki klient nie istnieje.");
                else
                {
                    var active = manager.Rentals.Where(r => r.CustomerId == searchCid && !r.IsReturned).ToList();
                    Console.WriteLine($"\nAktywne wypożyczenia ({active.Count}):");
                    foreach (var r in active)
                    {
                        var game = manager.Games.FirstOrDefault(g => g.Id == r.GameId);
                        Console.WriteLine($"ID: {r.Id} | Gra: {game?.Title} | Termin: {r.Deadline.ToShortDateString()}");
                    }
                }
                break;
            }

        case "5":
            {
                int rid = ReadInt("Podaj ID wypożyczenia do zwrotu: ");
                Console.WriteLine($"\n{manager.ReturnGame(rid)}");
                break;
            }

        case "6":
            {
                Console.WriteLine("\nLISTA ZAREJESTROWANYCH KLIENTÓW");

                if (!manager.Customers.Any())
                {
                    Console.WriteLine("Baza klientów jest pusta.");
                }
                else
                {
                    Console.WriteLine("ID".PadRight(5) + " | " + "IMIĘ I NAZWISKO".PadRight(25) + " | " + "ADRES EMAIL");
                    Console.WriteLine(new string('-', 60));

                    foreach (var c in manager.Customers)
                    {
                        string id = c.Id.ToString().PadRight(5);
                        string name = (c.FullName ?? "Brak danych").PadRight(25);
                        string email = c.Email ?? "Brak adresu";

                        Console.WriteLine($"{id} | {name} | {email}");
                    }
                }
                break;
            }

        case "7":
            {
                Console.WriteLine("\nDODAWANIE NOWEJ GRY");
                Console.WriteLine("Wybierz kategorię:");
                Console.WriteLine("1. Gra PC");
                Console.WriteLine("2. Gra strategiczna (PC)");
                Console.WriteLine("3. Gra na Konsolę");
                Console.WriteLine("4. Gra Retro (Konsola)");

                int type = ReadInt("Wybór (1-4): ");

                Console.Write("Tytuł gry: ");
                string title = Console.ReadLine() ?? "Brak tytułu";
                decimal rate = ReadDecimal("Stawka dzienna (zł): ");

                switch (type)
                {
                    case 1: // PC Game
                        {
                            Console.Write("Podaj model karty graficznej (GPU): ");
                            string gpu = Console.ReadLine() ?? "Brak danych";
                            int ram = ReadInt("Wymagany RAM (GB): ");
                            manager.AddGame(new PCGame { Title = title, BaseDailyRate = rate, MinimumGpu = gpu, RamRequired = ram });
                            break;
                        }

                    case 2: // Strategy Game
                        {
                            Console.Write("Podaj model karty graficznej (GPU): ");
                            string gpu = Console.ReadLine() ?? "Brak danych";
                            int ram = ReadInt("Wymagany RAM (GB): ");
                            Console.Write("Czy to strategia czasu rzeczywistego? (t/n): ");
                            bool rts = Console.ReadLine()?.ToLower() == "t";
                            manager.AddGame(new StrategyGame { Title = title, BaseDailyRate = rate, MinimumGpu = gpu, RamRequired = ram, IsRealTime = rts });
                            break;
                        }

                    case 3: // Console Game
                        {
                            Console.Write("Typ konsoli (np. PS5, Xbox): ");
                            string cType = Console.ReadLine() ?? "Inna";
                            manager.AddGame(new ConsoleGame { Title = title, BaseDailyRate = rate, ConsoleType = cType });
                            break;
                        }
                    case 4: // Retro Game
                        {
                            Console.Write("Typ konsoli: ");
                            string cType = Console.ReadLine() ?? "Inna";
                            int year = ReadInt("Rok wydania: ");
                            manager.AddGame(new RetroGame { Title = title, BaseDailyRate = rate, ConsoleType = cType, ReleaseYear = year });
                            break;
                        }
                    default:
                        Console.WriteLine("Nieprawidłowa kategoria!");
                        break;
                }

                manager.Save();
                Console.WriteLine("\nGra została pomyślnie dodana do katalogu!");
                break;
            }

        case "0":
            {
                Console.WriteLine("Zapisywanie danych...");
                manager.Save();
                return;
            }

        default:
            Console.WriteLine("Nieprawidłowa opcja.");
            break;
    }

    Console.WriteLine("\nNaciśnij dowolny klawisz, aby wrócić do menu...");
    Console.ReadKey();
}

static int ReadInt(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        if (int.TryParse(Console.ReadLine(), out int result)) return result;
        Console.WriteLine("Błąd: To nie jest poprawna liczba! Spróbuj ponownie.");
    }
}

static decimal ReadDecimal(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        if (decimal.TryParse(Console.ReadLine()?.Replace(".", ","), out decimal result)) return result;
        Console.WriteLine("Błąd: To nie jest poprawna kwota! (użyj przecinka)");
    }
}