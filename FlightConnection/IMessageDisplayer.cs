namespace FlightConnection
{
    interface IMessageDisplayer
    {
        void Send(string message);
        void Clear();
    }
}
