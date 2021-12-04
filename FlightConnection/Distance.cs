using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightConnection
{
    class Distance {
        public Distance(int distanceInKilometer) {
            if (distanceInKilometer <= 0) {
                throw new ArgumentOutOfRangeException("invalid distance value");
            }
            this.Kilometer = distanceInKilometer;
        }

        public static explicit operator Distance(int distanceInKilometer) {
            return new Distance(distanceInKilometer);
        }

        public int Kilometer { get; }

        public static Distance operator +(Distance lhs, Distance rhs) {
            return new Distance(lhs.Kilometer + rhs.Kilometer);
        }
    }
}
