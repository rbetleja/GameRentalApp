using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GameRentalApp.Models
{
    [JsonDerivedType(typeof(PCGame), typeDiscriminator: "pc")]
    [JsonDerivedType(typeof(StrategyGame), typeDiscriminator: "strategy")]
    [JsonDerivedType(typeof(ConsoleGame), typeDiscriminator: "console")]
    [JsonDerivedType(typeof(RetroGame), typeDiscriminator: "retro")]
    public abstract class GameItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal BaseDailyRate { get; set; }
        public bool IsAvailable { get; set; } = true;

        // Polimorfizm - wirtualna metoda obliczania kary
        public virtual decimal CalculateLateFee(int days) => BaseDailyRate * 2.0m * days;
    }
}