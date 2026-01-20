using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameRentalApp.Models
{
    public class StrategyGame : PCGame
    {
        public bool IsRealTime { get; set; } // t - RTS, n - Turn-based

        public override decimal CalculateLateFee(int days)
        {
            return (BaseDailyRate * 1.8m) * days;
        }
    }
}
