using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;

namespace FlightConnection
{
    interface IRowWritable
    {
        int WriteTo(IRow row, IDictionary<String, ICellStyle> styles, int startColumn);
    }
}