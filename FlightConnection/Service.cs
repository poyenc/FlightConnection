using System;

namespace FlightConnection
{
    class Service
    {
        public class Type
        {
            public Type(string type) {
                this.Value = type.Trim();
                if (Value.Length == 0) {
                    throw new ArgumentException(String.Format("cannot accept empty service type ({0})", type));
                }
            }

            public string Value { get; }
        }
    }
}
