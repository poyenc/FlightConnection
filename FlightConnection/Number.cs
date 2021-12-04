using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightConnection
{
    class Number
    {
        public Number(string number) {
            this.Value = number;
        }

        public static explicit operator Number(string number) {
            return new Number(number);
        }

        public string Value { get; }
    }
}
