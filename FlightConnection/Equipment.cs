using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightConnection
{
    class Equipment
    {
        public Equipment(string name) {
            this.Name = name;
        }

        public static explicit operator Equipment(string name) {
            return new Equipment(name);
        }

        public string Name { get; }
    }
}
