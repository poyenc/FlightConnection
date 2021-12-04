using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FlightConnection
{
    class AirportDistanceResolver
    {
        public AirportDistanceResolver(string filePath) {
            var extraDistances = new Dictionary<(Airport, Airport), Distance>();

            try {
                (IWorkbook workbook, FileStream _) = WorkbookFactory.Create(filePath, FileMode.Open, FileAccess.Read);
                ISheet sheet = workbook.GetSheetAt(0);

                foreach (IRow row in Enumerable.Range(1, sheet.LastRowNum).Select(index => sheet.GetRow(index))
                                                                                                               .TakeWhile(row => row != null && row.Count() == 3)) {
                    var from = (Airport)row.GetCell(0).StringCellValue;
                    var to = (Airport)row.GetCell(1).StringCellValue;
                    var distance = (Distance)Convert.ToInt32(row.GetCell(2).NumericCellValue);

                    var key = GetKey(from, to);
                    if (!extraDistances.ContainsKey(key)) {
                        extraDistances.Add(key, distance);
                    }
                }
            } catch (Exception) {
            }

            ExtraDistances = extraDistances;
        }

        private IReadOnlyDictionary<(Airport, Airport), Distance> ExtraDistances { get; }

        private static (Airport, Airport) GetKey(Airport first, Airport second) {
            return (first < second) ? (first, second) : (second, first);
        }

        public bool HasExtraDistance(Airport first, Airport second) {
            return ExtraDistances.ContainsKey(GetKey(first, second));
        }

        private Distance ExtraDistanceBetwen(Airport first, Airport second) {
            return ExtraDistances[GetKey(first, second)];
        }

        public Distance DistanceOf(Airport first, Airport second) {
            if (!HasExtraDistance(first, second)) {
                throw new ArgumentException(String.Format("cannot find distance data ({0}-{1}) in file", first.Code, second.Code));
            }

            return ExtraDistanceBetwen(first, second);
        }

        public Distance DistanceOf(Flight flight) {
            return flight.Schedule.Distance;
        }
    }
}
