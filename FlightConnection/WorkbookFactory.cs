using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.IO;

namespace FlightConnection
{
    class WorkbookFactory
    {
        static public (IWorkbook, FileStream) Create(String filePath, FileMode mode, FileAccess access) {
            FileStream stream = new FileStream(filePath, mode, access);
            if (access.HasFlag(FileAccess.Read)) {
                return (new XSSFWorkbook(stream), stream);
            } else {
                return (new XSSFWorkbook(), stream);
            }
        }
    }
}
