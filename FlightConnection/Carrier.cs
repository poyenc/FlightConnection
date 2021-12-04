using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightConnection
{
    class Carrier {
        public Carrier(string name) {
            this.Name = name;
        }

        public static explicit operator Carrier(string name) { 
            return new Carrier(name);
        }
        
        public string Name { get; }
    }
}
