using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship.Models
{
    class Player
    {
        public int ID { get; set; }
        public string name { get; set; }
        public DateTime date { get; set; }
        public int StatusID { get; set; }
    }
}
