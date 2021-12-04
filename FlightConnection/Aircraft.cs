using System;

namespace FlightConnection
{
    class Aircraft
    {
        public class Code
        {
            public Code(string number) {
                this.Value = number.Trim();
                if (Value.Length == 0) {
                    throw new ArgumentException(String.Format("cannot accept empty aircraft code ({0})", Value));
                }
            }

            public string Value { get; }
        }
    }
}
