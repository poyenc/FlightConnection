using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightConnection
{
    class FlightScheduleFieldResolver
    {
        public static DateTime ResolveDateTime(string dateString, string timeString) {
            var date = DateTime.Parse(dateString);
            var time = DateTime.Parse(timeString.Insert(2, ":"));

            return date.Add(time.TimeOfDay);
        }

        public static TimeSpan ResolveDuration(string durationString) {
            string[] tokens = durationString.Split(':');
            return new TimeSpan(Convert.ToInt32(tokens[0]), Convert.ToInt32(tokens[1]), 0);
        }
    }
}
