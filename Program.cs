using FlightRelay;
using Microsoft.FlightSimulator.SimConnect;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

class Program
{
    static SimConnectHandler? simHandler;

    static async Task Main()
    {
        simHandler = new SimConnectHandler();
        Console.Title = "Flight Relay";

        var api = new ApiConnection();

        simHandler.Connected += () => Console.WriteLine("Connected");
        simHandler.Disconnected += () => Console.WriteLine("Disconnected - is the simulator running?");
        simHandler.FlightDataReceived += async (data) =>

        {


            Console.SetCursorPosition(0, 2);
            int headingInt = (int)Math.Round(data.Heading) % 360; 
            string headingFormatted = headingInt.ToString("D3");    

            Console.WriteLine(
                $"DEBUG Altitude: {data.Altitude:F2} ft     " +
                $"Speed: {data.Speed:F2} knots     " +
                $"Vertical Speed: {data.VerticalSpeed:F0} fpm     " +
                $"Heading: {headingFormatted}°"
            );

            try
            {
                await api.SendFlightDataAsync(data);
                Console.WriteLine($"Sending flight data: Altitude={data.Altitude}, Speed={data.Speed}");

                Console.WriteLine("DEBUG: Flight data sent.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR sending flight data: {ex.Message}");
            }
        };

        var requestLoop = simHandler.StartRequestLoopAsync();

        while (!Console.KeyAvailable)
        {
            simHandler.ReceiveMessage();
            await Task.Delay(10);
        }

        simHandler.Dispose();
        Console.WriteLine("Connection closed.");
    }
}
