// See https://aka.ms/new-console-template for more information


using System.Net.Http.Headers;
using System.Net.Http.Json;

HttpClient client = new();
client.BaseAddress = new Uri("https://localhost:7165");
client.DefaultRequestHeaders.Accept.Clear();
client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

HttpResponseMessage response = await client.GetAsync("api/student");
response.EnsureSuccessStatusCode();
if (response.IsSuccessStatusCode)
    var student = await response.Content.ReadFromJsonAsync << IEnumerable <>> ();
