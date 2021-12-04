using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlightConnection
{
    class Longitude
    {
        public Longitude(String text) {
            Regex format = new Regex(@"(\d*)°(\d*)[']((\d*)[""]?)?([WE])");
            Match match = format.Match(text);
            if (!(match.Success && match.Groups.Count == 6)) {
                throw new ArgumentException(String.Format("unrecognized longitude string: {0}", text));
            }

            Degree = Convert.ToUInt16(match.Groups[1].Captures[0].Value);
            Debug.Assert(Degree < 180);
            Minutes = Convert.ToUInt16(match.Groups[2].Captures[0].Value);
            if (!String.IsNullOrEmpty(match.Groups[4].Captures[0].Value)) {
                Seconds = Convert.ToUInt16(match.Groups[4].Captures[0].Value);
            } else {
                Seconds = 0;
            }
            Hemisphere = (match.Groups[5].Captures[0].Value.Equals("W") ? Hemisphere.Western : Hemisphere.Eastern);
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
