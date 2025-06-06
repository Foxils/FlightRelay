using FlightRelay;
using System;
using System.Text;
using System.Text.Json;

namespace FlightRelay;

public class ApiConnection
{
    private readonly HttpClient _httpClient;
    
    public ApiConnection()
    {
        _httpClient = new HttpClient();

    }

    
    public async Task SendFlightDataAsync(FlightData data) // *
    {
        string json = JsonSerializer.Serialize(data); // *
        var content = new StringContent(json, Encoding.UTF8, "application/json"); //  *
        var response = await _httpClient.PostAsync("https://foxincode.info/apiflow", content); // *
        response.EnsureSuccessStatusCode(); // *
        Console.WriteLine("DEBUG: OK"); // *
        var key = ""; //  * API is not supported in this version, but it is here for reference.

    }


}


