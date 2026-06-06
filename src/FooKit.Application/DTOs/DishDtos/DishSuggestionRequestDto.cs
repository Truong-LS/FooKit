using FooKit.Domain.Enums;

namespace FooKit.Application.DTOs.DishDtos
{
    public class DishSuggestionRequestDto
    {
        public string Equipment { get; set; } = string.Empty;
        public DietaryType Diet { get; set; } = DietaryType.None;
        public decimal Budget { get; set; }
    }
}
