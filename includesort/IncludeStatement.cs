using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace includesort
{
    public class IncludeStatement
    {
        public string Name { get; set; }
        public char Symbol { get; set; }

        public IncludeStatement(string name, char symbol)
        {
            Name = name;
            Symbol = symbol;
        }
    }
}
