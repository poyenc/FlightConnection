using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightConnection
{
    interface IRowWritable
    {
        int WriteTo(IRow row, IDictionary<String, ICellStyle> styles, int startColumn);
    }
}