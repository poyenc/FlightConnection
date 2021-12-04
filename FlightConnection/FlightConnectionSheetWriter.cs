using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FlightConnection
{
    class FlightConnectionSheetWriter
    {
        class SheetHeader : IRowWritable
        {
            int IRowWritable.WriteTo(IRow row, IDictionary<string, ICellStyle> styles, int startColumn) {
                var flightCells = new string[] { "航空公司", "航班號", "出發機場", "到達機場", "出發日期", "出發時間",
                                                             "到達日期", "到達時間", "營運日期", "飛行時間", "距離(公里)", "中轉數量",
                                                             "機型", "班次", "座位數" };
                foreach (var (flightCell, index) in flightCells.Repeat(2).Select((element, index) => (element, index))) {
                    row.CreateCell(startColumn + index).SetCellValue(flightCell);
                }

                var connectionCells = new string[] { "轉機時間", "總旅行時間", "總距離", "總距離資料判斷", "DDT", "OOSAME", "RF", "QCI" };
                foreach (var (connectionCell, index) in connectionCells.Select((element, index) => (element, index))) {
                    row.CreateCell(startColumn + 2 * flightCells.Count() + index).SetCellValue(connectionCell);
                }

                return startColumn + 2 * flightCells.Count() + connectionCells.Count();
            }
        }

        private static IEnumerable<FlightConnection> OrderBy(IEnumerable<FlightConnection> connections, OrderingPolicy policy) {
            var sortedConnections = connections.OrderBy(connection => connection.Departure.ArrivalDateTime);
            switch (policy) {
                case OrderingPolicy.DayOfYear:
                    return sortedConnections;
                case OrderingPolicy.DayOfWeek:
                    int RedefinedDayOfWeekPriority(FlightConnection connection) => ((int)connection.Departure.ArrivalDateTime.DayOfWeek + 6) % 7;
                    return sortedConnections.OrderBy(RedefinedDayOfWeekPriority);
            }

            throw new InvalidOperationException(String.Format("meet invalid {0} enum value", policy.GetType().Name));
        }

        public static void Write(IEnumerable<FlightConnection> connections, String filePath, String sheetName, OrderingPolicy policy) {
            (IWorkbook workbook, FileStream stream) = WorkbookFactory.Create(filePath, FileMode.Create, FileAccess.Write);
            ISheet sheet = workbook.CreateSheet(sheetName);

            IDictionary<String, ICellStyle> styles = new Dictionary<String, ICellStyle>();
            {
                ICellStyle percent = workbook.CreateCellStyle();
                percent.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00%");
                styles.Add("percent", percent);

                ICellStyle count = workbook.CreateCellStyle();
                count.DataFormat = HSSFDataFormat.GetBuiltinFormat("#,##0");
                styles.Add("count", count);

                ICellStyle number = workbook.CreateCellStyle();
                number.DataFormat = HSSFDataFormat.GetBuiltinFormat("#,##0.00");
                styles.Add("number", number);
            }

            ((IRowWritable)new SheetHeader()).WriteTo(sheet.CreateRow(0), styles, 0);
            foreach (var (connection, index) in OrderBy(connections, policy).Select((element, index) => (element, index))) {
                ((IRowWritable)connection).WriteTo(sheet.CreateRow(index + 1), styles, 0);
            }

            workbook.Write(stream);
            stream.Close();
        }
    }
}
