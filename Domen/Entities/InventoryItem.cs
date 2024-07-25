using Domen.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domen.Entities
{
    public class InventoryItem
    {
        public InventoryItem()
        {
            Pos = new Point();
        }
        public string Name { get; set; }
        public Point Pos { get; set; }
        public int Amount { get; set; } = 1;
    }
}
