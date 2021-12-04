using System;

namespace FlightConnection
{
    class Country
    {
        public Country(string name) {
            this.Name = name.Trim();
            if (Name.Length == 0) {
                throw new ArgumentException(String.Format("cannot accept empty country name ({0})", name));
            }
        }

        public static bool operator ==(Country lhs, Country rhs) {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Country lhs, Country rhs) {
            return !(lhs == rhs);
        }

        public static bool operator <(Country lhs, Country rhs) {
            return String.Compare(lhs.Name, rhs.Name) < 0;
        }

        public static bool operator >(Country lhs, Country rhs) {
            return rhs < lhs;
        }

        public string Name { get; }

        public override string ToString() {
            return Name.ToString();
        }

        public override bool Equals(object other) {
            if (other == null || !this.GetType().Equals(other.GetType())) {
                return false;
            }

            return Equals((Country)other);
        }

        public override int GetHashCode() {
            return Name.GetHashCode();
        }

        public bool Equals(Country other) {
            return Name.Equals(other.Name);
        }

        public int CompareTo(Country other) {
            return Name.CompareTo(other.Name);
        }
    }
}
