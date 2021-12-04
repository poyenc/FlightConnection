using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;

namespace FlightConnection
{
    class FlightConnection : IRowWritable
    {
        public FlightConnection(Flight arrival, Flight departure, AirportDistanceResolver distanceResolver) {
            if (arrival.Schedule.Destination != departure.Schedule.Origin) {
                throw new ArgumentException(
                    String.Format("destination ({0}) of arrival flight and origin ({1}) of departure flight is not same",
                        arrival.Schedule.Destination, departure.Schedule.Origin));
            }

            if (departure.DepartureDateTime <= arrival.DestinationArrivalDateTime) {
                throw new ArgumentException(
                    String.Format("invalid arrival time ({0}) & departure time ({1}) for flight connection",
                        arrival.DestinationArrivalDateTime, departure.DepartureDateTime));
            }

            Arrival = arrival;
            Departure = departure;
            DistanceResolver = distanceResolver;
        }

        public Flight Arrival { get; }

        public Flight Departure { get; }

        private AirportDistanceResolver DistanceResolver { get; }

        public TimeSpan TransitTime {
            get {
                return Departure.DepartureDateTime - Arrival.DestinationArrivalDateTime;
            }
        }

        public TimeSpan TotalTravelTime {
            get {
                return TransitTime + Arrival.Schedule.Duration + Departure.Schedule.Duration;
            }
        }

        public TimeSpan DirectTravelTime {
            get {
                return TimeSpan.FromMinutes(40 + 0.068 * Distance.Kilometer);
            }
        }

        public Distance Distance {
            get {
                if (DistanceResolver.HasExtraDistance(Arrival.Schedule.Origin, Departure.Schedule.Destination)) {
                    return DistanceResolver.DistanceOf(Arrival.Schedule.Origin, Departure.Schedule.Destination);
                }

                return DistanceResolver.DistanceOf(Arrival) + DistanceResolver.DistanceOf(Departure);
            }
        }

        int IRowWritable.WriteTo(IRow row, IDictionary<string, ICellStyle> styles, int startColumn) {
            int departureStartColumn = ((IRowWritable)Arrival).WriteTo(row, styles, startColumn);
            int connectionStartColumn = ((IRowWritable)Departure).WriteTo(row, styles, departureStartColumn);

            row.CreateCell(connectionStartColumn).SetCellValue(TransitTime.ToString(@"hh\:mm"));
            row.CreateCell(connectionStartColumn + 1).SetCellValue(TotalTravelTime.ToString(@"hh\:mm"));
            row.CreateCell(connectionStartColumn + 2).SetCellValue(Distance.Kilometer);
            if (DistanceResolver.HasExtraDistance(Arrival.Schedule.Origin, Departure.Schedule.Destination)) {
                row.CreateCell(connectionStartColumn + 3).SetCellValue("V");
            }
            row.CreateCell(connectionStartColumn + 4).SetCellValue(DirectTravelTime.ToString(@"hh\:mm"));
            if (Arrival.Schedule.Origin.Equals(Departure.Schedule.Destination)) {
                row.CreateCell(connectionStartColumn + 5).SetCellValue("V");
            }

            var rf = (float)TotalTravelTime.TotalMinutes / DirectTravelTime.TotalMinutes;
            row.CreateCell(connectionStartColumn + 6).SetCellValue(rf);

            // var qci = (1 < rf && rf < 1.5) ? ((1.5 - rf) / 0.5) : 0; // old rule before 2021.02.16
            var qci = (1.5 - rf) / 0.5; // rule update on 2021.02.17
            row.CreateCell(connectionStartColumn + 7).SetCellValue(qci);

            return connectionStartColumn + 8;
        }
    }
}
