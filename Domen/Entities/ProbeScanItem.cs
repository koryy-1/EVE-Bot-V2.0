﻿using Domen.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domen.Entities
{
    public class ProbeScanItem
    {
        public Point Pos { get; set; }
        public string ID { get; set; }
        public Distance Distance { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
    }
}
