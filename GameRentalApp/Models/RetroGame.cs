using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameRentalApp.Models
{
    public class RetroGame : ConsoleGame
    {
        public int ReleaseYear { get; set; }

        public override decimal CalculateLateFee(int days) => BaseDailyRate * 4.0m * days;
    }
}