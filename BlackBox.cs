using System.Runtime.InteropServices;

namespace FlightRelay
{
    public enum DataRequests { Request1 }
    public enum DataDefinitions { FlightData }
    enum DATA_DEFINITIONS { FLIGHT_DATA }

    // Data store

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct FlightData
    {
        public double Altitude;
        public double Speed;
        public double VerticalSpeed;
        public double Heading;

        public double Latitude;
        public double Longitude;
        public double TrueAirspeed;
        public double WindSpeed;
        public double WindDirection;
        public double AmbientTemperature;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string WaypointIdent;

    }
}
