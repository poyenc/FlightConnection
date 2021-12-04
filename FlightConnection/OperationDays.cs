using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FlightConnection
{
    class OperationDays : IEnumerable<DayOfWeek>
    {
        public OperationDays(string daysString, DayOfWeek firstDay) {
            string nonSpaceDaysString = String.Concat(daysString.Where(c => !Char.IsWhiteSpace(c)));

            Match m = Regex.Match(nonSpaceDaysString, @"[1234567]+");
            if (!m.Success) {
                throw new ArgumentOutOfRangeException(String.Format("invalid operation days string: {0}", daysString));
            }

            ISet<DayOfWeek> days = new SortedSet<DayOfWeek>();
            foreach (char c in nonSpaceDaysString) {
                if (!days.Add(ToDayOfWeek(c))) {
                    throw new ArgumentException(String.Format("meet duplicated day in operation days string: {0}", daysString));
                }
            }

            if (days.Count == 0) {
                throw new ArgumentException(String.Format("invalid operation days string: {0}", daysString));
            }

            bool BeforeFirstDay(DayOfWeek dayOfWeek) => dayOfWeek < firstDay;
            Days = days.SkipWhile(BeforeFirstDay).Concat(days.TakeWhile(BeforeFirstDay));
        }

        private static DayOfWeek ToDayOfWeek(char c) {
            return (DayOfWeek)((c - '1' + 1) % 7);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable<DayOfWeek>)this).GetEnumerator();
        }

        IEnumerator<DayOfWeek> IEnumerable<DayOfWeek>.GetEnumerator() {
            return Days.GetEnumerator();
        }

        private IEnumerable<DayOfWeek> Days { get; }

        public override string ToString() {
            char ToChar(DayOfWeek dayOfWeek) => (Days.Contains(dayOfWeek) ? (char)('1' + ((int)dayOfWeek + 6) % 7) : ' ');

            var builder = new StringBuilder();

            var values = EnumUtil.GetValues<DayOfWeek>();
            var adjustedValues = values.Skip(1).Concat(values.Take(1));
            foreach (var dayChar in adjustedValues.Select(ToChar)) {
                builder.Append(dayChar, 1);
            }

            return builder.ToString().Trim();
        }
    }
}
