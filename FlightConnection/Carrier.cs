using System;

namespace FlightConnection
{
    class Carrier
    {
        public Carrier(string name) {
            this.Name = name.Trim();
            if (Name.Length == 0) {
                throw new ArgumentException(String.Format("cannot accept empty carrier name ({0})", name));
            }
        }

        public static explicit operator Carrier(string name) {
            var trimmedName = name.Trim();
            return 0 < trimmedName.Length ? new Carrier(trimmedName) : null;
        }

        public string Name { get; }
    }
}
