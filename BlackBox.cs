using System.Runtime.InteropServices;

namespace FlightRelay
{
    public enum DataRequests { Request1 }
    public enum DataDefinitions { FlightData }
    enum DATA_DEFINITIONS { FLIGHT_DATA }

   

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct FlightData
    {
        public double Altitude { get; set; }
        public double Speed { get; set; }
        public double VerticalSpeed { get; set; }
        public double Heading { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double TrueAirspeed { get; set; }
        public double WindSpeed { get; set; }
        public double WindDirection { get; set; }
        public double AmbientTemperature { get; set; }
      
    }
}
