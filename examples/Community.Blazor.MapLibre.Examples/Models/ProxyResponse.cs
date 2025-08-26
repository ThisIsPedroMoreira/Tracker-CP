namespace Community.Blazor.MapLibre.Examples.WebAssembly.Models
{
    public class ProxyResponse
    {
        public string contents { get; set; } = default!;    
        public ProxyStatusResponse status { get; set; } = default!;
    }

    public class ProxyStatusResponse
    {
        public long content_length { get; set; } = default!;
        public string content_type { get; set; } = default!;
        public long http_code { get; set; } = default!;
        public long response_time { get; set; } = default!;
        public string url { get; set; } = default!;

    }
}
