using System;
using System.Text.RegularExpressions;

namespace FlightConnection
{
    class Coordinate
    {
        public Coordinate(String text) {
            Regex format = new Regex(@"([^\s]*)[\s]*([^\s]*)");
            Match match = format.Match(text.Trim());
            if (!(match.Success && match.Groups.Count == 3)) {
                throw new ArgumentException(String.Format("unrecognized coordinate string: {0}", text));
            }

            Latitude = new Latitude(match.Groups[1].Captures[0].Value);
            Longitude = new Longitude(match.Groups[2].Captures[0].Value);
        }

        public Latitude Latitude { get; }

        public Longitude Longitude { get; }
    }
}
