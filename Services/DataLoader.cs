using AnnoCalculator.Models;
using System.Text.Json;

namespace AnnoCalculator.Services
{
    public class DataLoader(HttpClient httpClient)
    {
        public async Task<ProductionData?> LoadDataAsync(string url = "data/Anno117.json")
        {
            Stream stream = await httpClient.GetStreamAsync(url);

            ProductionData? data = await JsonSerializer.DeserializeAsync<ProductionData>(stream, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return data;
        }
    }
}
