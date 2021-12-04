using System;
using System.Linq;

namespace FlightConnection
{
    class TwoWayIndicator
    {
        public TwoWayIndicator(UInt16 indicator) {
            Value = indicator;
            if (!new uint[] { 0, 1 }.Contains(Value)) {
                throw new ArgumentException(String.Format("cannot convert {0} to the 2-way indicator", indicator));
            }
        }

        public static explicit operator TwoWayIndicator(UInt16 indicator) {
            return new TwoWayIndicator(indicator);
        }

        public UInt16 Value { get; }
    }
}
