using System.Text.Json.Serialization;

namespace aifordevs
{
    public class ApiResponse
    {
        public string id { get; set; }

        [JsonPropertyName("object")]
        public string Object { get; set; }
        public long created { get; set; }
        public string model { get; set; }
        public Choice[] choices { get; set; }
        public Usage usage { get; set; }
        public string system_fingerprint { get; set; }
    }

    public class Choice
    {
        public int index { get; set; }
        public Message message { get; set; }
        public object logprobs { get; set; } // null or detailed object, depending on API config
        public string finish_reason { get; set; }
    }

    public class Message
    {
        public string role { get; set; }
        public string content { get; set; }
    }

    public class Usage
    {
        public int prompt_tokens { get; set; }
        public int completion_tokens { get; set; }
        public int total_tokens { get; set; }
    }


}
