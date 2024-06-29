namespace aifordevs
{
    public class ApiRequest
    {
        public string model { get; set; }
        public ResponseFormat response_format { get; set; }
        public List<RequestMessage> messages { get; set; }
    }

    public class ResponseFormat
    {
        public string type { get; set; }
    }

    public class RequestMessage
    {
        public string role { get; set; }
        public string content { get; set; }
    }
}
