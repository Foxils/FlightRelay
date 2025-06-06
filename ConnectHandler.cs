using Microsoft.FlightSimulator.SimConnect;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace FlightRelay
{
    public class SimConnectHandler : IDisposable
    {
        private SimConnect? simconnect; 
        private bool isConnected;
        private bool isMSFS2024 = true; // Assume 2024 until proven otherwise will be utilized in the future versions.
        private bool hasShownConnectionLostMessage = false;

        public FlightData CurrentFlightData { get; private set; }

        public event Action? Connected;
        public event Action? Disconnected;
        public event Action<FlightData>? FlightDataReceived;

        public SimConnectHandler()
        {
            TryConnect(); 
        }

        private void TryConnect()
        {
            int retryDelayMs = 8000; 
            while (true)
            {
                try
                {
                    simconnect = new SimConnect("Managed Data Request", IntPtr.Zero, 0, null, 0); // IntPtr.Zero workaround for window handle.

                    RegisterFlightDataDefinition();
                    simconnect.RegisterDataDefineStruct<FlightData>(DataDefinitions.FlightData);

                    simconnect.OnRecvOpen += OnRecvOpen;
                    simconnect.OnRecvQuit += OnRecvQuit;
                    simconnect.OnRecvSimobjectDataBytype += OnRecvSimobjectDataBytype;

                    Console.Clear(); 
                    isConnected = true;
                    hasShownConnectionLostMessage = false;
                    break; 
                }
                catch (COMException)
                {
                    Console.WriteLine("Initialization failed. Is the simulator running? Retrying in 8 seconds.");
                    Thread.Sleep(retryDelayMs);
                }
            }
        }

        public void ReceiveMessage()
        {
            if (!isConnected)
            {
                TryReconnect(); 
                return;
            }

            try
            {
                simconnect?.ReceiveMessage();
                hasShownConnectionLostMessage = false;
            }
            catch (COMException)
            {
                if (!hasShownConnectionLostMessage)
                {
                    Console.WriteLine("The connection was lost or the simulator is not running. Attempting to reconnect...");
                    hasShownConnectionLostMessage = true;
                }
                isConnected = false;
                TryReconnect();
            }
        }

        private void TryReconnect()
        {
            simconnect?.Dispose(); 
            simconnect = null; 
            TryConnect(); 
        }

        private void OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            isConnected = true;
            Connected?.Invoke();
        }

        private void OnRecvQuit(SimConnect sender, SIMCONNECT_RECV data)
        {
            isConnected = false;
            Disconnected?.Invoke();
        }

        private void OnRecvSimobjectDataBytype(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data)
        {
            if (data.dwRequestID == (uint)DataRequests.Request1)
            {
                FlightData flightData = (FlightData)data.dwData[0];
                CurrentFlightData = flightData;
                FlightDataReceived?.Invoke(flightData);
            }
        }

        public async Task StartRequestLoopAsync()
        {
            while (true)
            {
                if (isConnected)
                {
                    try
                    {
                        simconnect?.RequestDataOnSimObjectType(
                            DataRequests.Request1,
                            DataDefinitions.FlightData,
                            0,
                            SIMCONNECT_SIMOBJECT_TYPE.USER);
                    }
                    catch (COMException)
                    {
                        isConnected = false;
                        if (!hasShownConnectionLostMessage)
                        {
                            Console.WriteLine("Failed to request data. Connection may be lost.");
                            hasShownConnectionLostMessage = true;
                        }
                    }
                }

                await Task.Delay(1000);
            }
        }

        private void RegisterFlightDataDefinition() // Start of Simconnect data definition
        {
            void Add(string name, string units, SIMCONNECT_DATATYPE type = SIMCONNECT_DATATYPE.FLOAT64) =>
                simconnect!.AddToDataDefinition(DataDefinitions.FlightData, name, units, type, 0, SimConnect.SIMCONNECT_UNUSED);

            Add("PLANE ALTITUDE", "feet");
            Add("AIRSPEED INDICATED", "knots");
            Add("VERTICAL SPEED", "feet per minute");
            Add("PLANE HEADING DEGREES MAGNETIC", "degrees");
            Add("PLANE LATITUDE", "degrees");
            Add("PLANE LONGITUDE", "degrees");
            Add("AIRSPEED TRUE", "knots");
            Add("AMBIENT WIND VELOCITY", "knots");
            Add("AMBIENT WIND DIRECTION", "degrees");
            Add("AMBIENT TEMPERATURE", "celsius");

            simconnect!.AddToDataDefinition(DataDefinitions.FlightData, "GPS WP NEXT ID", null, SIMCONNECT_DATATYPE.STRING256, 0, SimConnect.SIMCONNECT_UNUSED);
        }

        public void Dispose()
        {
            simconnect?.Dispose();
            simconnect = null;
        }
    }
}