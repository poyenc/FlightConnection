using NPOI.SS.UserModel;

namespace FlightConnection
{
    interface IRowReadable
    {
        void ReadFrom(IRow row, int startColumn);
    }
}
