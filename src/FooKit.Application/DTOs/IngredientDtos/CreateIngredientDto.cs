namespace FooKit.Application.DTOs.IngredientDtos
{
    public class CreateIngredientDto
    {
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal DefaultPrice { get; set; } = 0;
        public int EstimatedUses { get; set; } = 1;
    }
}
