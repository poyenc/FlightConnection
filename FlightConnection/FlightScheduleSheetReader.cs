using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FlightConnection
{
    class FlightScheduleSheetReader
    {
        public static (IReadOnlyList<FlightSchedule>, String) Read(string filePath) {
            (IWorkbook workbook, FileStream _) = WorkbookFactory.Create(filePath, FileMode.Open, FileAccess.Read);
            ISheet sheet = workbook.GetSheetAt(0);

            var schedules = new List<FlightSchedule>();
            foreach (IRow row in Enumerable.Range(2, sheet.LastRowNum).Select(index => sheet.GetRow(index))
                                                                                                           .TakeWhile(row => row != null)) {
                FlightSchedule schedule = new FlightSchedule();
                ((IRowReadable)schedule).ReadFrom(row, 0);

                schedules.Add(schedule);
            }

            return (schedules, sheet.SheetName);
        }
    }
}
