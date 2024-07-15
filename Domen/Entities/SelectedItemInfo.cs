using Domen.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domen.Entities
{
    public class SelectedItemInfo
    {
        public string Name { get; set; }
        public Distance Distance { get; set; }
        public List<SelectedItemButton> Buttons { get; set; }
    }
}
