using System;
using System.Collections.Generic;
using FooKit.Application.DTOs.DishDtos;

namespace FooKit.Application.DTOs.HomepageDtos
{
    public class MealSuggestionResponseDto
    {
        public bool IsPremiumExpired { get; set; } = false;
        public List<SuggestedDishDto> Dishes { get; set; } = new List<SuggestedDishDto>();
    }
}
