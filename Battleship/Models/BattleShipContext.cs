using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Battleship.Models
{
    class BattleShipContext : DbContext
    {
        public BattleShipContext() : base("DefaultConnection") { }
        public DbSet<Status> statuses { get; set; }
        public DbSet<Player> players { get; set; }
    }
}
