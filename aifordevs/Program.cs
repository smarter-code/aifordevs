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
            Console.WriteLine("--------------------------------------");
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
        return $"I want you to tell me the key traits of {countryName} in terms of weather, location and natural resources.Give short and concise answers.Do not use markdown format. Give the answer with JSON containing three string properties (weather, location and naturalResources).";
    }

    // Handles the API call
    private static async Task<string> GetApiResponse(string prompt)
    {
        ApiRequest apiRequest = new ApiRequest
        {
            model = "gpt-4o",
            response_format = new ResponseFormat { type = "json_object" },
            messages = new List<RequestMessage>
            {
                new RequestMessage { role = "system", content = "You are a geography expert. You always respond in JSON." },
                new RequestMessage { role = "user", content = prompt }
            }
        };

        string requestBody = JsonSerializer.Serialize(apiRequest, new JsonSerializerOptions { WriteIndented = true });
        var content = new StringContent(requestBody, System.Text.Encoding.UTF8, "application/json");
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
        if (response != null && response.choices.Length > 0 && response.choices[0].message != null)
        {
            return JsonSerializer.Deserialize<CountryTraits>(response.choices[0].message.content);
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
        Console.WriteLine($"Location: {traits.location}");
        Console.WriteLine($"Weather: {traits.weather}");
        Console.WriteLine($"Natural Resources: {traits.naturalResources}");
    }
}
