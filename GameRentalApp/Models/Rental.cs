using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameRentalApp.Models
{
    public class Rental
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public int CustomerId { get; set; }
        public DateTime RentalDate { get; set; }
        public DateTime Deadline { get; set; }
        public bool IsReturned { get; set; }
    }
}