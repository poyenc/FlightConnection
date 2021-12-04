using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlightConnection
{
    public partial class Form1 : Form, IMessageDisplayer {
        public Form1() {
            InitializeComponent();
        }

        private static IEnumerable<Airport> GetAirports(string airportCodes) {
            foreach (Airport airport in airportCodes.Split(',')
                                                                         .Select(airportCode => airportCode.Trim())
                                                                         .Where(airportCode => !string.IsNullOrEmpty(airportCode))
                                                                         .Select(airportCode => new Airport(airportCode))) {
                yield return airport;
            }
        }

        private string ArrivalFlightScheduleSeet {
            get { return textBox1.Text; }
            set { textBox1.Text = value; }
        }

        private string DepartureFlightScheduleSeet {
            get { return textBox2.Text; }
            set { textBox2.Text = value; }
        }

        private string AirportDistanceSheet {
            get { return textBox8.Text; }
            set { textBox8.Text = value; }
        }

        private string AirportCoordinateSheet {
            get { return textBox10.Text; }
            set { textBox10.Text = value; }
        }

        private string FlightConnectionSheet {
            get {
                if (textBox4.Text.Trim() != String.Empty) {
                    if (Path.GetExtension(textBox4.Text.Trim()) == String.Empty) {
                        return textBox4.Text + ".xlsx";
                    }
                }
               return textBox4.Text;
            }
        }

        private OrderingPolicy Policy {
            get { return checkBox1.Checked ? OrderingPolicy.DayOfWeek : OrderingPolicy.DayOfYear; }
        }

        private static readonly TimeSpan DefaultTransitTime = TimeSpan.FromMinutes(90);

        private TimeSpan MinTransitTime {
            get {
                try {
                    return TimeSpan.FromMinutes(Convert.ToInt32(textBox3.Text.Trim()));
                } catch (FormatException) {
                    MessageDisplayer.Send(
                       String.Format("detect invalid concent ({0}), use default transit time ({1}) instead",
                            textBox3.Text.Trim(), DefaultTransitTime.Minutes));
                    return DefaultTransitTime;
                }
            }
        }

        private TimeSpan MaxTransitTime {
            get {
                try {
                    return TimeSpan.FromMinutes(Convert.ToInt32(textBox9.Text.Trim()));
                } catch (FormatException) {
                    MessageDisplayer.Send(
                        String.Format("detect invalid concent ({0}), use default transit time ({1}) instead",
                            textBox9.Text.Trim(), DefaultTransitTime.Minutes));
                    return DefaultTransitTime;
                }
            }
        }

        private string OriginAirportCodes {
            get { return String.Join(",", textBox5.Lines).Trim().Replace(Environment.NewLine, ""); }
        }

        private string DestinationAirportCodes {
            get { return String.Join(",", textBox6.Lines).Trim().Replace(Environment.NewLine, ""); }
        }

        private IMessageDisplayer MessageDisplayer {
            get { return (IMessageDisplayer) this; }
        }

        public void Send(string message) {
            textBox7.AppendText(message);
            textBox7.AppendText(Environment.NewLine);
        }

        public void Clear() {
            textBox7.Clear();
        }

        private void OnNumericTextBoxKeyPress(object sender, KeyPressEventArgs e) {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) {
                e.Handled = true;
            }
        }

        private void button4_Click(object sender, EventArgs e) {
            MessageDisplayer.Clear();
            MessageDisplayer.Send("computation started");

            try {
                AirportTimeZoneResolver.SetFile(AirportCoordinateSheet);

                var (arrivals, arrivalsSheetName) = FlightScheduleSheetReader.Read(ArrivalFlightScheduleSeet);
                var (departures, departuresSheetName) = FlightScheduleSheetReader.Read(DepartureFlightScheduleSeet);

                IEnumerable<Airport> origins = GetAirports(OriginAirportCodes);
                MessageDisplayer.Send(
                    String.Format("origins: {0}", String.Join(", ", origins.Select(airport => airport.ToString()))));

                IEnumerable<Airport> destinations = GetAirports(DestinationAirportCodes);
                MessageDisplayer.Send(
                    String.Format("destinations: {0}", String.Join(", ", destinations.Select(airport => airport.ToString()))));

                var distanceResolver = new AirportDistanceResolver(AirportDistanceSheet);
                var filter = new FlightConnectionFilter(arrivals, departures, distanceResolver, origins, destinations);

                IEnumerable<FlightConnection> connections = filter.Filter(MinTransitTime, MaxTransitTime);
                FlightConnectionSheetWriter.Write(connections, FlightConnectionSheet, departuresSheetName, Policy);

                MessageDisplayer.Send("computation completed");
            } catch (Exception exception) {
                MessageDisplayer.Send(exception.Message);
                MessageDisplayer.Send("computation aborted");
            }
        }


        private void button3_Click(object sender, EventArgs e) {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog()) {
                saveFileDialog.Filter = "Excel 2007 (*.xlsx) |*.xlsx";
                saveFileDialog.FilterIndex = 2;
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK && saveFileDialog.FileName != "") {
                    textBox4.Text = saveFileDialog.FileName;
                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e) {
            using (OpenFileDialog openFileDialog = new OpenFileDialog()) {
                openFileDialog.Filter = "Excel 2007 (*.xlsx) |*.xlsx";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK) {
                    ArrivalFlightScheduleSeet = openFileDialog.FileName;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            using (OpenFileDialog openFileDialog = new OpenFileDialog()) {
                openFileDialog.Filter = "Excel 2007 (*.xlsx) |*.xlsx";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK) {
                    DepartureFlightScheduleSeet = openFileDialog.FileName;
                }
            }
        }

        private void button5_Click(object sender, EventArgs e) {
            using (OpenFileDialog openFileDialog = new OpenFileDialog()) {
                openFileDialog.Filter = "Excel 2007 (*.xlsx) |*.xlsx";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK) {
                    AirportDistanceSheet = openFileDialog.FileName;
                }
            }
        }

        private void button6_Click(object sender, EventArgs e) {
            using (OpenFileDialog openFileDialog = new OpenFileDialog()) {
                openFileDialog.Filter = "Excel 2007 (*.xlsx) |*.xlsx";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK) {
                    AirportCoordinateSheet = openFileDialog.FileName;
                }
            }
        }
    }
}
