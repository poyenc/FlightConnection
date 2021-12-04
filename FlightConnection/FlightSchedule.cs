using NPOI.SS.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightConnection
{
    class FlightSchedule : IEnumerable<Flight>, IRowReadable
    {
        public Carrier Carrier { get; private set; }

        public Number Number { get; private set; }

        public Airport Origin { get; private set; }

        public Airport Destination { get; private set; }

        public DateTime FirstDepartureDateTime { get; private set; }

        public TimeSpan Duration { get; private set; }

        public TimeSpan TimeZoneDifference { get; private set; }

        public Distance Distance { get; private set; } 

        public int Stops { get; private set; }

        public Equipment Equipment { get; private set; }

        public int Frequency { get; private set; }

        public int Seats { get; private set; }

        public DateTime FirstArrivalDateTime {
            get { return FirstDepartureDateTime + Duration; }
        }

        public DateTime FirstDestinationArrivalDateTime {
            get { return FirstArrivalDateTime + TimeZoneDifference; }
        }

        public OperationDays OperationDays { get; private set; }

        void IRowReadable.ReadFrom(IRow row, int startColumn) {
            Carrier = (Carrier)row.GetCell(startColumn).StringCellValue;
            Number = (Number)row.GetCell(startColumn + 1).StringCellValue;
            Origin = (Airport)row.GetCell(startColumn + 2).StringCellValue;
            Destination = (Airport)row.GetCell(startColumn + 3).StringCellValue;
            FirstDepartureDateTime = FlightScheduleFieldResolver.ResolveDateTime(
                row.GetCell(startColumn + 13).StringCellValue, row.GetCell(startColumn + 4).StringCellValue);
            OperationDays = new OperationDays(row.GetCell(startColumn + 6).StringCellValue, FirstDepartureDateTime.DayOfWeek);
            if (!OperationDays.Contains(FirstDepartureDateTime.DayOfWeek)) {
                throw new ArgumentException("operation days is not matched to the flight schedule");
            }

            Duration = FlightScheduleFieldResolver.ResolveDuration(row.GetCell(startColumn + 7).StringCellValue);
            Distance = (Distance) Convert.ToInt32(row.GetCell(startColumn + 8).StringCellValue);
            Stops = Convert.ToInt32(row.GetCell(startColumn + 9).NumericCellValue);
            Equipment = (Equipment) row.GetCell(startColumn + 10).StringCellValue;
            Frequency = Convert.ToInt32(row.GetCell(startColumn + 11).NumericCellValue);
            Seats = Convert.ToInt32(row.GetCell(startColumn + 12).NumericCellValue);

            DateTime referenceArrivalDateTime = FlightScheduleFieldResolver.ResolveDateTime(
                row.GetCell(startColumn + 13).StringCellValue, row.GetCell(startColumn + 5).StringCellValue);
            TimeZoneDifference = referenceArrivalDateTime - FirstArrivalDateTime;
            // adjust time zone different by time zone offset when they are conflict
            if (AirportTimeZoneResolver.HasTimeZoneOffset(Origin) && AirportTimeZoneResolver.HasTimeZoneOffset(Destination)) {
                bool isFlightToEast = (0 < AirportTimeZoneResolver.TimeZoneOffset(Destination) - AirportTimeZoneResolver.TimeZoneOffset(Origin));
                if (TimeZoneDifference.Hours < -10 && isFlightToEast) {
                    TimeZoneDifference += TimeSpan.FromHours(24);
                } else if (10 < TimeZoneDifference.Hours && !isFlightToEast) {
                    TimeZoneDifference -= TimeSpan.FromHours(24);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable<Flight>) this).GetEnumerator();
        }

        IEnumerator<Flight> IEnumerable<Flight>.GetEnumerator() {
            int GetDayStep(DayOfWeek prevDay, DayOfWeek nextDay) => ((int)nextDay + 7 - (int)prevDay) % 7;

            var daySteps = Enumerable.Zip(
              OperationDays, OperationDays.Cycle().Skip(1), GetDayStep
            ).Take(OperationDays.Count() - 1);

            bool InSameYear(int dayOffset) => FirstDepartureDateTime.Year == FirstDepartureDateTime.AddDays(dayOffset).Year;

            yield return new Flight(this, 0);
            foreach (int dayOffset in daySteps.CumulativeSum(0, (sum, daySetp) => sum + daySetp).TakeWhile(InSameYear)) {
                yield return new Flight(this, dayOffset);
            }
        }
    }
}
