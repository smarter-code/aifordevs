using aifordevs;
using System.Text.Json;

class Program
{
    private static readonly HttpClient client = new HttpClient();
    // Represents the country traits
    public class CountryTraits
    {
        public string location { get; set; }
        public string weather { get; set; }
        public string naturalResources { get; set; }
    }

    static async Task Main(string[] args)
    {
        InitializeHttpClient();

        while (true)
        {
            Console.WriteLine("Enter a country name or type 'stop' to exit:");
            string country = Console.ReadLine().Trim();

            if (country.ToLower() == "stop")
                break;

            string prompt = GeneratePrompt(country);
            string jsonResponse = await GetApiResponse(prompt);
            CountryTraits countryTraits = ProcessResponse(jsonResponse);
            DisplayCountryTraits(countryTraits);
        }
    }

    // Set up HttpClient headers only once
    private static void InitializeHttpClient()
    {
        client.DefaultRequestHeaders.Add("Authorization", "Bearer <APIKey>");
    }

    // Generates the prompt string
    private static string GeneratePrompt(string countryName)
    {
        return $"I want you to tell me the key traits of {countryName} in terms of weather, location and natural resources. Give short and concise answers. Do not use markdown format";
    }

    // Handles the API call
    private static async Task<string> GetApiResponse(string prompt)
    {
        var content = new StringContent($"{{\r\n    \"model\": \"gpt-4o\",\r\n    \r\n    \r\n    \"presence_penalty\": -2,\r\n    \"messages\": [\r\n      {{\r\n        \"role\": \"system\",\r\n        \"content\": \"You are a geography expert.\"\r\n      }},\r\n      {{\r\n        \"role\": \"user\",\r\n        \"content\": \"{prompt}\"\r\n      }}\r\n    ]\r\n  }}", System.Text.Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions")
        {
            Content = content
        };

        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    // Process the API response
    private static CountryTraits ProcessResponse(string jsonResponse)
    {
        ApiResponse response = JsonSerializer.Deserialize<ApiResponse>(jsonResponse);
        if (response != null && response.Choices.Length > 0 && response.Choices[0].Message != null)
        {
            return JsonSerializer.Deserialize<CountryTraits>(response.Choices[0].Message.Content);
        }
        return null;
    }

    // Display the deserialized data
    private static void DisplayCountryTraits(CountryTraits traits)
    {
        if (traits == null)
        {
            Console.WriteLine("No data to display.");
            return;
        }
        Console.WriteLine($"Location: {traits.Location}");
        Console.WriteLine($"Weather: {traits.Weather}");
        Console.WriteLine($"Natural Resources: {traits.NaturalResources}");
    }
}
