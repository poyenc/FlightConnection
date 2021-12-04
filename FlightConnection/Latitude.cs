using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace FlightConnection
{
    class Latitude
    {
        public Latitude(String text) {
            Regex format = new Regex(@"(\d*)°(\d*)[']((\d*)[""]?)?([NS])");
            Match match = format.Match(text);
            if (!(match.Success && match.Groups.Count == 6)) {
                throw new ArgumentException(String.Format("unrecognized latitude string: {0}", text));
            }

            Degree = Convert.ToUInt16(match.Groups[1].Captures[0].Value);
            Debug.Assert(Degree < 180);
            Minutes = Convert.ToUInt16(match.Groups[2].Captures[0].Value);
            if (!String.IsNullOrEmpty(match.Groups[4].Captures[0].Value)) {
                Seconds = Convert.ToUInt16(match.Groups[4].Captures[0].Value);
            } else {
                Seconds = 0;
            }
            Hemisphere = (match.Groups[5].Captures[0].Value.Equals("N") ? Hemisphere.Northern : Hemisphere.Southern);
        }

        public UInt16 Degree {
            get; private set;
        }

        public UInt16 Minutes {
            get; private set;
        }

        public UInt16 Seconds {
            get; private set;
        }

        public Hemisphere Hemisphere {
            get; private set;
        }
    }
}
