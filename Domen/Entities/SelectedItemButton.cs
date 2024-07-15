using Domen.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domen.Entities
{
    public class SelectedItemButton
    {
        //public SpaceObjectAction Action { get; set; }
        public string Action { get; set; }
        public Point Pos { get; set; }
        public bool IsEnable { get; set; }
    }
}
