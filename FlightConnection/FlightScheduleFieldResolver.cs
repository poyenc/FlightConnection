using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlightConnection
{
    class FlightScheduleFieldResolver
    {
        public static Carrier ResolveCarrier(ICell cell) {
            return (Carrier)cell.StringCellValue;
        }

        public static Airport ResolveAirport(ICell cell) {
            return new Airport(cell.StringCellValue);
        }

        public static Country ResolveCountry(ICell cell) {
            return new Country(cell.StringCellValue);
        }

        public static Flight.Number ResolveFlightNumber(ICell cell) {
            return new Flight.Number((UInt16)cell.NumericCellValue);
        }

        public static Service.Type ResolveServiceType(ICell cell) {
            return new Service.Type(cell.StringCellValue);
        }

        public static DateTime ResolveDate(ICell cell) {
            return DateTime.ParseExact(cell.StringCellValue, "dd/MM/yyyy", null);
        }

        public static TimeSpan ResolveTime(ICell cell) {
            return cell.DateCellValue.TimeOfDay;
        }

        public static TimeSpan ResolveTimeOffset(ICell cell) {
            return new TimeSpan(((Int16)cell.NumericCellValue / 100), 0, 0);
        }

        public static IEnumerable<Airport> ResolveAirports(ICell cell) {
            return cell.StringCellValue.Split('!').Where(airportString => 0 < airportString.Length).Select(airportString => new Airport(airportString));
        }

        public static Aircraft.Code ResolveAircraftCode(ICell cell) {
            if (cell.CellType == CellType.String) {
                return new Aircraft.Code(cell.StringCellValue);
            }

            return new Aircraft.Code(cell.NumericCellValue.ToString());
        }

        public static Distance ResolveDistance(ICell cell) {
            return new Distance((int)cell.NumericCellValue);
        }

        public static TimeSpan ResolveDuration(ICell cell) {
            return new TimeSpan(0, (int)cell.NumericCellValue, 0);
        }

        public static UInt16 ResolveStops(ICell cell) {
            return (UInt16)cell.NumericCellValue;
        }

        public static ThreeWayIndicator ResolveThreeWayIndicator(ICell cell) {
            return new ThreeWayIndicator((Int16)cell.NumericCellValue);
        }

        public static TwoWayIndicator ResolveTwoWayIndicator(ICell cell) {
            return new TwoWayIndicator((UInt16)cell.NumericCellValue);
        }

        public static Frequency ResolveFrequency(ICell cell) {
            return new Frequency((UInt16)cell.NumericCellValue);
        }

        public static OperationDays ResolveOperationDays(ICell cell, DayOfWeek firstDay) {
            if (cell.CellType == CellType.Numeric) {
                return new OperationDays(cell.NumericCellValue.ToString(), firstDay);
            }

            return new OperationDays(cell.StringCellValue, firstDay);
        }
    }
}
