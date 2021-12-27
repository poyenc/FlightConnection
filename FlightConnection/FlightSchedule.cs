using NPOI.SS.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FlightConnection
{
    class FlightSchedule : IEnumerable<Flight>, IRowReadable
    {
        public Carrier Carrier { get; private set; }

        public Flight.Number FlightNumber { get; private set; }

        public Service.Type ServiceType { get; private set; }

        public OperationDays OperationDays { get; private set; }

        public DateTime EffectiveDate { get; private set; }

        public DateTime DiscontinuedDate { get; private set; }

        public Airport DepartureAirport { get; private set; }

        public Country DepartureCountry { get; private set; }

        public TimeSpan DepartureTime { get; private set; }

        public TimeSpan DepartureTimeOffset { get; private set; }

        public Airport ArrivalAirport { get; private set; }

        public Country ArrivalCountry { get; private set; }

        public TimeSpan ArrivalTime { get; private set; }

        public TimeSpan ArrivalTimeOffset { get; private set; }

        public Aircraft.Code AircraftCode { get; private set; }

        public ThreeWayIndicator ArrivalDayIndicator { get; private set; }

        public UInt16 Stops { get; private set; }

        public ICollection<Airport> StopAirports { get; private set; }

        public Distance Distance { get; private set; }

        public TimeSpan Duration { get; private set; }

        public TimeSpan LayoverTime { get; private set; }

        public Carrier CodeShareCarrier { get; private set; }

        public TwoWayIndicator CodeShareIndicator { get; private set; }

        public TwoWayIndicator WetLeaseIndicator { get; private set; }

        public string CodeShareInfo { get; private set; }

        public string WetLeaseInfo { get; private set; }

        public Frequency Frequency { get; private set; }

        public Frequency WeeklyFrequency { get; private set; }

        void IRowReadable.ReadFrom(IRow row, int startColumn) {
            Carrier = FlightScheduleFieldResolver.ResolveCarrier(row.GetCell(startColumn + 0));
            FlightNumber = FlightScheduleFieldResolver.ResolveFlightNumber(row.GetCell(startColumn + 1));
            ServiceType = FlightScheduleFieldResolver.ResolveServiceType(row.GetCell(startColumn + 2));
            EffectiveDate = FlightScheduleFieldResolver.ResolveDate(row.GetCell(startColumn + 3));
            DiscontinuedDate = FlightScheduleFieldResolver.ResolveDate(row.GetCell(startColumn + 4));
            OperationDays = FlightScheduleFieldResolver.ResolveOperationDays(row, 5, EffectiveDate.DayOfWeek);
            DepartureAirport = FlightScheduleFieldResolver.ResolveAirport(row.GetCell(startColumn + 12));
            DepartureCountry = FlightScheduleFieldResolver.ResolveCountry(row.GetCell(startColumn + 15));
            DepartureTime = FlightScheduleFieldResolver.ResolveTime(row.GetCell(startColumn + 16));
            DepartureTimeOffset = FlightScheduleFieldResolver.ResolveTimeOffset(row.GetCell(startColumn + 18));
            ArrivalAirport = FlightScheduleFieldResolver.ResolveAirport(row.GetCell(startColumn + 20));
            ArrivalCountry = FlightScheduleFieldResolver.ResolveCountry(row.GetCell(startColumn + 23));
            ArrivalTime = FlightScheduleFieldResolver.ResolveTime(row.GetCell(startColumn + 24));
            ArrivalTimeOffset = FlightScheduleFieldResolver.ResolveTimeOffset(row.GetCell(startColumn + 26));
            AircraftCode = FlightScheduleFieldResolver.ResolveAircraftCode(row.GetCell(startColumn + 28));
            ArrivalDayIndicator = FlightScheduleFieldResolver.ResolveThreeWayIndicator(row.GetCell(startColumn + 33));
            Stops = FlightScheduleFieldResolver.ResolveStops(row.GetCell(startColumn + 34));
            StopAirports = FlightScheduleFieldResolver.ResolveAirports(row.GetCell(startColumn + 35)).ToList();
            Distance = FlightScheduleFieldResolver.ResolveDistance(row.GetCell(startColumn + 40));
            Duration = FlightScheduleFieldResolver.ResolveDuration(row.GetCell(startColumn + 41));
            LayoverTime = FlightScheduleFieldResolver.ResolveDuration(row.GetCell(startColumn + 42));
            CodeShareCarrier = FlightScheduleFieldResolver.ResolveCarrier(row.GetCell(startColumn + 45));
            CodeShareIndicator = FlightScheduleFieldResolver.ResolveTwoWayIndicator(row.GetCell(startColumn + 46));
            WetLeaseIndicator = FlightScheduleFieldResolver.ResolveTwoWayIndicator(row.GetCell(startColumn + 47));
            CodeShareInfo = FlightScheduleFieldResolver.ResolveInfo(row.GetCell(startColumn + 48));
            WetLeaseInfo = FlightScheduleFieldResolver.ResolveInfo(row.GetCell(startColumn + 49));
            Frequency = FlightScheduleFieldResolver.ResolveFrequency(row.GetCell(startColumn + 55));
            WeeklyFrequency = FlightScheduleFieldResolver.ResolveFrequency(row.GetCell(startColumn + 56));
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable<Flight>)this).GetEnumerator();
        }

        IEnumerator<Flight> IEnumerable<Flight>.GetEnumerator() {
            int GetDayStep(DayOfWeek prevDay, DayOfWeek nextDay) => ((int)nextDay + 7 - (int)prevDay) % 7;

            var daySteps = (OperationDays.Count() == 1 ? 7.Yield().Cycle() : Enumerable.Zip(
              OperationDays, OperationDays.Cycle().Skip(1), GetDayStep
            ).Take(OperationDays.Count()).Cycle());

            bool isValid(int dayOffset) => EffectiveDate.AddDays(dayOffset) <= DiscontinuedDate;

            yield return new Flight(this, 0);
            foreach (int dayOffset in daySteps.CumulativeSum(0, (sum, daySetp) => sum + daySetp).TakeWhile(isValid)) {
                yield return new Flight(this, dayOffset);
            }
        }
    }
}
