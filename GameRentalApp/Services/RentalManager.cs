using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using GameRentalApp.Models;

namespace GameRentalApp.Services;

public class RentalManager
{
    public List<GameItem> Games { get; set; } = new();
    public List<Customer> Customers { get; set; } = new();
    public List<Rental> Rentals { get; set; } = new();
    private const string FileName = "database.json";

    public void LoadData()
    {
        try
        {
            if (!File.Exists(FileName)) return;

            string json = File.ReadAllText(FileName);

            var loadedData = JsonSerializer.Deserialize<RentalManager>(json);

            if (loadedData != null)
            {
                this.Games = loadedData.Games ?? new List<GameItem>();
                this.Customers = loadedData.Customers ?? new List<Customer>();
                this.Rentals = loadedData.Rentals ?? new List<Rental>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd podczas wczytywania bazy: {ex.Message}");
        }
    }

    public void AddGame(GameItem g) { g.Id = Games.Count > 0 ? Games.Max(x => x.Id) + 1 : 1; Games.Add(g); }
    public void AddCustomer(Customer c) { c.Id = Customers.Count > 0 ? Customers.Max(x => x.Id) + 1 : 1; Customers.Add(c); }

    public string RentGame(int gId, int cId, int days)
    {
        var game = Games.FirstOrDefault(g => g.Id == gId);
        if (game == null) return "BŁĄD: Gra o takim ID nie istnieje.";
        if (!game.IsAvailable) return $"BŁĄD: Gra '{game.Title}' jest już wypożyczona.";

        var customer = Customers.FirstOrDefault(c => c.Id == cId);
        if (customer == null) return "BŁĄD: Klient o takim ID nie istnieje.";

        if (days <= 0) return "BŁĄD: Okres wypożyczenia musi być dłuższy niż 0 dni.";

        game.IsAvailable = false;
        Rentals.Add(new Rental
        {
            Id = Rentals.Count > 0 ? Rentals.Max(x => x.Id) + 1 : 1,
            GameId = gId,
            CustomerId = cId,
            RentalDate = DateTime.Now,
            Deadline = DateTime.Now.AddDays(days)
        });
        return $"SUKCES: Wypożyczono grę '{game.Title}' klientowi {customer.FullName}.";
    }

    public string ReturnGame(int rId)
    {
        var rental = Rentals.FirstOrDefault(r => r.Id == rId && !r.IsReturned);
        if (rental == null) return "BŁĄD: Nie znaleziono aktywnego wypożyczenia o tym ID.";

        var game = Games.FirstOrDefault(g => g.Id == rental.GameId);
        if (game != null) game.IsAvailable = true;
        rental.IsReturned = true;

        if (DateTime.Now > rental.Deadline)
        {
            int days = (DateTime.Now - rental.Deadline).Days;
            if (days == 0) days = 1;
            decimal fee = game?.CalculateLateFee(days) ?? 0;
            return $"ZWRÓCONO Z OPÓŹNIENIEM! ({days} dni). Należy naliczyć karę: {fee:C2}";
        }
        return "SUKCES: Gra zwrócona w terminie.";
    }

    public void Save()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(FileName, JsonSerializer.Serialize(this, options));
        }
        catch (Exception ex) { Console.WriteLine($"Błąd zapisu: {ex.Message}"); }
    }
}