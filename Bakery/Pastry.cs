using System;
using System.Collections.Generic;
using System.Text;

namespace BakeryProject
{
    public class Pastry
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public Pack[] Packs { get; set; }
    }
}