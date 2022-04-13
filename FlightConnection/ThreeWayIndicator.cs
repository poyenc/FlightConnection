using System;
using System.Linq;

namespace FlightConnection
{
    class ThreeWayIndicator
    {
        public ThreeWayIndicator(Int16 indicator) {
            Value = indicator;
            if (!(Value == -1 || 0 <= Value)) {
                throw new ArgumentException(String.Format("cannot convert {0} to the 3-way indicator", indicator));
            }
        }

        public static explicit operator ThreeWayIndicator(Int16 indicator) {
            return new ThreeWayIndicator(indicator);
        }

        public Int16 Value { get; }
    }
}
