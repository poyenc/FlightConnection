namespace FlightConnection
{
    class Carrier
    {
        public Carrier(string name) {
            this.Name = name;
        }

        public static explicit operator Carrier(string name) {
            return new Carrier(name);
        }

        public string Name { get; }
    }
}
