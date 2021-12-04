using System;

namespace FlightConnection
{
    class Frequency
    {
        public Frequency(UInt16 frequency) {
            this.Value = frequency;
            if (5 * 365 < Value) {
                throw new ArgumentException(String.Format("frequency value ({0}) is too large", frequency));
            }
        }

        public static explicit operator Frequency(UInt16 frequency) {
            return new Frequency(frequency);
        }

        public UInt16 Value { get; }
    }
}
