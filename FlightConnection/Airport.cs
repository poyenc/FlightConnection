using System;

namespace FlightConnection
{
    class Airport : IEquatable<Airport>, IComparable<Airport>
    {
        public Airport(string code) {
            this.Code = code.Trim();
        }

        public static explicit operator Airport(string code) {
            return new Airport(code);
        }

        public static bool operator ==(Airport lhs, Airport rhs) {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Airport lhs, Airport rhs) {
            return !(lhs == rhs);
        }

        public static bool operator <(Airport lhs, Airport rhs) {
            return String.Compare(lhs.Code, rhs.Code) < 0;
        }

        public static bool operator >(Airport lhs, Airport rhs) {
            return rhs < lhs;
        }

        public string Code { get; }

        public override string ToString() {
            return Code.ToString();
        }

        public override bool Equals(object other) {
            if (other == null || !this.GetType().Equals(other.GetType())) {
                return false;
            }

            return Equals((Airport)other);
        }

        public override int GetHashCode() {
            return Code.GetHashCode();
        }

        public bool Equals(Airport other) {
            return Code.Equals(other.Code);
        }

        public int CompareTo(Airport other) {
            return Code.CompareTo(other.Code);
        }
    }
}
