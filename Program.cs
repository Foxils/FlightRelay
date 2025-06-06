using Microsoft.FlightSimulator.SimConnect;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using FlightRelay;

class Program
{
    static SimConnectHandler? simHandler;

    static async Task Main()
    {
        simHandler = new SimConnectHandler();
        Console.Title = "Flight Relay";
        simHandler.Connected += () => Console.WriteLine("Connected");
        simHandler.Disconnected += () => Console.WriteLine("Disconnected - is the simulator running?");
        simHandler.FlightDataReceived += (data) =>

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
