using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace FlightConnection
{
    class AirportTimeZoneResolver
    {
        public AirportTimeZoneResolver(string filePath) {
            var coordinates = new Dictionary<Airport, Coordinate>();

            try {
                (IWorkbook workbook, FileStream _) = WorkbookFactory.Create(filePath, FileMode.Open, FileAccess.Read);
                ISheet sheet = workbook.GetSheetAt(0);

                foreach (IRow row in Enumerable.Range(1, sheet.LastRowNum).Select(index => sheet.GetRow(index))
                                                                                                               .TakeWhile(row => row != null && row.Count() == 2)) {
                    var airport = (Airport)row.GetCell(0).StringCellValue;
                    var coordinate = new Coordinate(row.GetCell(1).StringCellValue);

                    if (!coordinates.ContainsKey(airport)) {
                        coordinates.Add(airport, coordinate);
                    }
                }
            } catch (Exception) {
            }

            Coordinates = coordinates;
        }

        private IReadOnlyDictionary<Airport, Coordinate> Coordinates { get; }

        public static bool SetFile(string filePath) {
            if (Instance != null) {
                return false;
            }

            Instance = new AirportTimeZoneResolver(filePath);
            return true;
        }

        private static AirportTimeZoneResolver Instance { get; set; }

        public static bool HasTimeZoneOffset(Airport airport) {
            return Instance.Coordinates.ContainsKey(airport);
        }

        public static int TimeZoneOffset(Airport airport) {
            Debug.Assert(HasTimeZoneOffset(airport));

            Longitude longitude = Instance.Coordinates[airport].Longitude;
            return (int)((longitude.Hemisphere == Hemisphere.Western ? -1 : 1) * Math.Floor((longitude.Degree + 7.5) / 15));
        }
    }
}
