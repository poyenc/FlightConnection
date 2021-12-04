using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightConnection
{
    class Flight : IRowWritable
    {
        public Flight(FlightSchedule schedule, int dayOffset) {
         
            if (dayOffset < 0 ||
                !schedule.OperationDays.Contains(GetDayOfWeek(schedule.FirstDepartureDateTime.DayOfWeek, dayOffset))) {
                throw new ArgumentOutOfRangeException(String.Format("invalid day offset({0}) for flight", dayOffset));
            }

            this.DayOffset = dayOffset;
            this.Schedule = schedule;
        }

        private static DayOfWeek GetDayOfWeek(DayOfWeek dayOfWeek, int dayOffset) {
            return (DayOfWeek)(((int) dayOfWeek + dayOffset % 7) % 7);
        }

        int IRowWritable.WriteTo(IRow row, IDictionary<string, ICellStyle> styles, int startColumn) {
            row.CreateCell(startColumn).SetCellValue(Schedule.Carrier.Name);
            row.CreateCell(startColumn + 1).SetCellValue(Schedule.Number.Value);
            row.CreateCell(startColumn + 2).SetCellValue(Schedule.Origin.Code);
            row.CreateCell(startColumn + 3).SetCellValue(Schedule.Destination.Code);
            row.CreateCell(startColumn + 4).SetCellValue(DepartureDateTime.Date.ToString("yyyy-MM-dd"));
            row.CreateCell(startColumn + 5).SetCellValue(DepartureDateTime.TimeOfDay.ToString("hhmm"));
            row.CreateCell(startColumn + 6).SetCellValue(DestinationArrivalDateTime.Date.ToString("yyyy-MM-dd"));
            row.CreateCell(startColumn + 7).SetCellValue(DestinationArrivalDateTime.TimeOfDay.ToString("hhmm"));
            row.CreateCell(startColumn + 8).SetCellValue(Schedule.OperationDays.ToString());
            row.CreateCell(startColumn + 9).SetCellValue(Schedule.Duration.ToString(@"hh\:mm"));
            row.CreateCell(startColumn + 10).SetCellValue(Schedule.Distance.Kilometer);
            row.CreateCell(startColumn + 11).SetCellValue(Schedule.Stops);
            row.CreateCell(startColumn + 12).SetCellValue(Schedule.Equipment.Name);
            row.CreateCell(startColumn + 13).SetCellValue(Schedule.Frequency);
            row.CreateCell(startColumn + 14).SetCellValue(Schedule.Seats);

            return startColumn + 15;
        }

        public int DayOffset { get; }

        public DateTime DepartureDateTime {
            get {
                return Schedule.FirstDepartureDateTime.AddDays(DayOffset);
            }
        }

        public DateTime ArrivalDateTime { 
            get { 
                return DepartureDateTime + Schedule.Duration; 
            } 
        }

        public DateTime DestinationArrivalDateTime {
            get {
                return ArrivalDateTime + Schedule.TimeZoneDifference; 
            } 
        }

        public FlightSchedule Schedule { get; }
    }
}
