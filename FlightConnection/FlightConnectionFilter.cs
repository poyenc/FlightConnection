using System;
using System.Collections.Generic;
using System.Linq;

namespace FlightConnection
{
    class FlightConnectionFilter
    {
        public FlightConnectionFilter(IEnumerable<FlightSchedule> arrival, IEnumerable<FlightSchedule> departure,
                                                    AirportDistanceResolver distanceResolver, IEnumerable<Airport> origins,
                                                    IEnumerable<Airport> destinations) {
            if (arrival.Count() == 0 || departure.Count() == 0) {
                throw new ArgumentException("no possible flight connection can be found");
            }

            if (arrival.Any(schedule => schedule.ArrivalAirport != arrival.First().ArrivalAirport) ||
                departure.Any(scheulde => scheulde.DepartureAirport != departure.First().DepartureAirport)) {
                throw new ArgumentException("flight schedules do not have same origin/desitination");
            }

            if (arrival.First().ArrivalAirport != departure.First().DepartureAirport) {
                throw new ArgumentException(
                    String.Format("find mismatch arrival/departure flights",
                        arrival.First().ArrivalAirport.Code, departure.First().ArrivalAirport.Code));
            }

            ISet<Airport> originAirports = new SortedSet<Airport>(origins);
            Arrivals = new List<Flight>(arrival.Where(schedule => originAirports.Count == 0 || originAirports.Contains(schedule.DepartureAirport))
                                                               .SelectMany(shedule => shedule.AsEnumerable())
                                                               .OrderBy(flight => flight.DestinationArrivalDateTime));

            ISet<Airport> destinationAirports = new SortedSet<Airport>(destinations);
            Departures = new List<Flight>(departure.Where(schedule => destinationAirports.Count == 0 || destinationAirports.Contains(schedule.ArrivalAirport))
                                                                           .SelectMany(shedule => shedule.AsEnumerable())
                                                                           .OrderBy(flight => flight.DepartureDateTime));

            DistanceResolver = distanceResolver;
        }

        private IReadOnlyList<Flight> Arrivals { get; }

        private IReadOnlyList<Flight> Departures { get; }

        private AirportDistanceResolver DistanceResolver { get; }

        private IEnumerable<FlightConnection> FilterImpl(IReadOnlyList<Flight> arrivals, IReadOnlyList<Flight> departures,
                                                                                     AirportDistanceResolver distanceResolver, TimeSpan minTransitTime,
                                                                                     TimeSpan maxTransitTime) {
            if (arrivals.Count == 0 || departures.Count == 0) {
                yield break;
            }

            int iDeparture = 0;
            foreach (Flight arrival in arrivals) {
                bool Valid(Flight departure) => arrival.DestinationArrivalDateTime < departure.DepartureDateTime;
                iDeparture = departures.FindIndex(iDeparture, Valid);
                if (iDeparture < 0) {
                    yield break;
                }

                bool Connectable(Flight departure) {
                    var transitTime = departure.DepartureDateTime - arrival.DestinationArrivalDateTime;
                    return minTransitTime <= transitTime && transitTime <= maxTransitTime;
                };
                foreach (Flight departure in departures.From(iDeparture).TakeWhile(Connectable)) {
                    yield return new FlightConnection(arrival, departure, distanceResolver);
                }
            }
        }

        public IEnumerable<FlightConnection> Filter(TimeSpan minTransitTime, TimeSpan maxTransitTime) {
            return FilterImpl(Arrivals, Departures, DistanceResolver, minTransitTime, maxTransitTime);
        }
    }
}
