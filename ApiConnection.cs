using FlightRelay;
using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FlightRelay;

[JsonSerializable(typeof(FlightData))]
public partial class FlightDataJsonContext : JsonSerializerContext
{
}

public class ApiConnection
{
    private readonly HttpClient _httpClient;
    
    public ApiConnection()
    {
        _httpClient = new HttpClient();
    }

    public async Task SendFlightDataAsync(FlightData data)
    {
        var options = FlightDataJsonContext.Default.FlightData;

        string json = JsonSerializer.Serialize(data, options); 
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        Console.WriteLine($"DEBUG: JSON being sent: {json}");
        var response = await _httpClient.PostAsync("http://localhost:7186/api/Flightdata", content);
        response.EnsureSuccessStatusCode();
        Console.WriteLine("DEBUG: OK");
    }
}


