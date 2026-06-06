namespace FooKit.Application.Configuration
{
    public class GeminiOptions
    {
        public const string SectionName = "GeminiOptions";

        public string BaseUrl { get; set; } = "https://generativelanguage.googleapis.com";
        public string ApiKey { get; set; } = string.Empty;
        public string Model { get; set; } = "gemini-1.5-flash";
    }
}
