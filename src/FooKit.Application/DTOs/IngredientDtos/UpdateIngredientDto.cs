namespace FooKit.Application.DTOs.IngredientDtos
{
    public class UpdateIngredientDto
    {
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal DefaultPrice { get; set; }
        public int EstimatedUses { get; set; }
    }
}
