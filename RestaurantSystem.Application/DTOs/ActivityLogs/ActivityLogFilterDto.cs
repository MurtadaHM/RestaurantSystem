using System;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.DTOs.ActivityLogs
{
    public class ActivityLogFilterDto
    {
        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        public Guid? UserId { get; set; }

        public string? UserName { get; set; }

        public string? Module { get; set; }

        public ActivityActionType? ActionType { get; set; }

        public string? Search { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 50;
    }
}