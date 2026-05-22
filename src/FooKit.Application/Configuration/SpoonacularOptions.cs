namespace MyProject.Application.Configuration
{
    public class SpoonacularOptions
    {
        public const string SectionName = "SpoonacularOptions";

        public string BaseUrl { get; set; } = "https://api.spoonacular.com";
        public string ApiKey { get; set; } = string.Empty;
    }
}
