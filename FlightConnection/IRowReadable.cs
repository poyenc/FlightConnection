using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightConnection
{
    interface IRowReadable
    {
        void ReadFrom(IRow row, int startColumn);
    }
}
