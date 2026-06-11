using System.Collections.Generic;
using FooKit.Domain.Enums;

namespace FooKit.Application.DTOs.UserDtos
{
    public class SaveDietaryProfileRequestDto
    {
        public List<DietaryType> Diets { get; set; } = new List<DietaryType>();
        public List<string> Allergies { get; set; } = new List<string>();
        public List<string> FavoriteCuisines { get; set; } = new List<string>();
        public decimal? WeeklyBudget { get; set; }
    }
}
