using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlightConnection
{
    class Flight : IRowWritable
    {
        public class Number
        {
            public Number(UInt16 number) {
                this.Value = number;
            }

            public UInt16 Value { get; }
        }

        public class Header : IRowWritable
        {
            public int WriteTo(IRow row, IDictionary<string, ICellStyle> styles, int startColumn) {
                var cells = new string[] {
                    "carrier", "flightnumber",    "serviceType", "effectiveDate",   "discontinuedDate", "departureDate",
                    "day1",    "day2",    "day3",    "day4",    "day5",    "day6",    "day7",   "departureAirport",
                    "departureCountry",   "departureTimePub", "departureUTCVariance", "arrivalAirport", "arrivalCountry", "arrivalTimePub",
                    "arrivalUTCVariance", "subAircraftCode", "flightArrivalDayIndicator", "stops",  "stopCodes", "flightDistance",    "elapsedTime", "layoverTime",
                    "SSIMcodeShareCarrier",    "codeshareIndicator",  "wetleaseIndicator",   "codeshareInfo",   "wetleaseInfo",
                    "daysOfOperation", "totalFrequency",  "weeklyFrequency"
                };
                foreach (var (cell, index) in cells.WithIndex()) {
                    row.CreateCell(startColumn + index).SetCellValue(cell);
                }
                return startColumn + cells.Length;
            }
        }

        public Flight(FlightSchedule schedule, int dayOffset) {
            this.DayOffset = dayOffset;
            this.Schedule = schedule;
        }

        int IRowWritable.WriteTo(IRow row, IDictionary<string, ICellStyle> styles, int startColumn) {
            row.CreateCell(startColumn).SetCellValue(Schedule.Carrier.Name);
            row.CreateCell(startColumn + 1).SetCellValue(Schedule.FlightNumber.Value);
            row.CreateCell(startColumn + 2).SetCellValue(Schedule.ServiceType.Value);
            row.CreateCell(startColumn + 3).SetCellValue(Schedule.EffectiveDate.ToString("yyyy/MM/dd"));
            row.CreateCell(startColumn + 4).SetCellValue(Schedule.DiscontinuedDate.ToString("yyyy/MM/dd"));
            row.CreateCell(startColumn + 5).SetCellValue(DepartureLocalDateTime.Date.ToString("yyyy/MM/dd"));
            Schedule.OperationDays.WriteTo(row, styles, startColumn + 6);
            row.CreateCell(startColumn + 13).SetCellValue(Schedule.DepartureAirport.Code);
            row.CreateCell(startColumn + 14).SetCellValue(Schedule.DepartureCountry.Name);
            row.CreateCell(startColumn + 15).SetCellValue(Schedule.DepartureTime.ToString(@"hh\:mm\:ss"));
            row.CreateCell(startColumn + 16).SetCellValue(Schedule.DepartureTimeOffset.Hours * 100);
            row.CreateCell(startColumn + 17).SetCellValue(Schedule.ArrivalAirport.Code);
            row.CreateCell(startColumn + 18).SetCellValue(Schedule.ArrivalCountry.Name);
            row.CreateCell(startColumn + 19).SetCellValue(Schedule.ArrivalTime.ToString(@"hh\:mm\:ss"));
            row.CreateCell(startColumn + 20).SetCellValue(Schedule.ArrivalTimeOffset.Hours * 100);
            row.CreateCell(startColumn + 21).SetCellValue(Schedule.AircraftCode.Value);
            row.CreateCell(startColumn + 22).SetCellValue(Schedule.ArrivalDayIndicator.Value);
            row.CreateCell(startColumn + 23).SetCellValue(Schedule.Stops);
            row.CreateCell(startColumn + 24).SetCellValue(String.Join("!", Enumerable.Select(Schedule.StopAirports, (Airport airport) => airport.Code).ToArray()));
            row.CreateCell(startColumn + 25).SetCellValue(Schedule.Distance.Kilometer);
            row.CreateCell(startColumn + 26).SetCellValue(Schedule.Duration.Minutes);
            row.CreateCell(startColumn + 27).SetCellValue(Schedule.LayoverTime.Minutes);
            row.CreateCell(startColumn + 28).SetCellValue(Schedule.CodeShareCarrier != null ? Schedule.CodeShareCarrier.Name : String.Empty);
            row.CreateCell(startColumn + 29).SetCellValue(Schedule.CodeShareIndicator.Value);
            row.CreateCell(startColumn + 30).SetCellValue(Schedule.WetLeaseIndicator.Value);
            row.CreateCell(startColumn + 31).SetCellValue(Schedule.CodeShareInfo);
            row.CreateCell(startColumn + 32).SetCellValue(Schedule.WetLeaseInfo);
            row.CreateCell(startColumn + 33).SetCellValue(Schedule.OperationDays.ToString());
            row.CreateCell(startColumn + 34).SetCellValue(Schedule.Frequency.Value);
            row.CreateCell(startColumn + 35).SetCellValue(Schedule.WeeklyFrequency.Value);

            return startColumn + 36;
        }

        public int DayOffset { get; }

        public DateTime DepartureLocalDateTime {
            get {
                return Schedule.EffectiveDate.AddDays(DayOffset) + Schedule.DepartureTime;
            }
        }

        public DateTime DepartureDateTime {
            get {
                return DepartureLocalDateTime - Schedule.DepartureTimeOffset;
            }
        }

        public DateTime ArrivalDateTime {
            get {
                return DepartureDateTime + Schedule.Duration;
            }
        }

        public DateTime ArrivalLocalDateTime {
            get {
                return ArrivalDateTime + Schedule.ArrivalTimeOffset;
            }
        }

        public FlightSchedule Schedule { get; }
    }
}
