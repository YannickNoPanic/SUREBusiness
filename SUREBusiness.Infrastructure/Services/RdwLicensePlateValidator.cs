using SUREBusiness.Core.Interfaces;
using System.Text.Json;

namespace SUREBusiness.Infrastructure.Services;

public sealed class RdwLicensePlateValidator(HttpClient httpClient) : ILicensePlateValidator
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<bool> IsValidAsync(string licensePlate)
    {
        if (string.IsNullOrWhiteSpace(licensePlate))
            return false;

        var normalized = new string(licensePlate
            .Where(char.IsLetterOrDigit)
            .ToArray())
            .ToUpperInvariant();

        if (normalized.Length == 0)
            return false;

        var url = $"https://opendata.rdw.nl/resource/m9d7-ebf2.json?kenteken={normalized}";

        using var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        await using var stream = await response.Content.ReadAsStreamAsync();
        using var doc = await JsonDocument.ParseAsync(stream);

        return doc.RootElement.ValueKind == JsonValueKind.Array
            && doc.RootElement.GetArrayLength() > 0;
    }
}
