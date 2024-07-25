using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domen.Entities
{
    public class BotConfig
    {
        public string BaseApiUrl { get; set; }
        public string Nickname { get; set; }
        public string ShipName { get; set; }
        public int DronesInSpaceCount { get; set; }
        public int WeaponRange { get; set; }
    }
}
