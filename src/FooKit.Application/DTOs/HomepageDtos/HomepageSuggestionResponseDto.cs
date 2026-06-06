using System;
using System.Collections.Generic;
using FooKit.Application.DTOs.DishDtos;

namespace FooKit.Application.DTOs.HomepageDtos
{
    public class HomepageSuggestionResponseDto
    {
        public bool IsPremiumExpired { get; set; } = false;
        public List<SuggestedDishDto> Breakfast { get; set; } = new List<SuggestedDishDto>();
        public List<SuggestedDishDto> Lunch { get; set; } = new List<SuggestedDishDto>();
        public List<SuggestedDishDto> Dinner { get; set; } = new List<SuggestedDishDto>();
    }
}
