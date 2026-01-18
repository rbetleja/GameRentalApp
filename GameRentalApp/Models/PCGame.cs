using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameRentalApp.Models
{
    public class PCGame : GameItem
    {
        public string MinimumGpu { get; set; }
        public int RamRequired { get; set; }
    }
}