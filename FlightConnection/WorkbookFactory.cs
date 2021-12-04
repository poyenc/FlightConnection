using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
