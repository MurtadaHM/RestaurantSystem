using System;
using System.Collections.Generic;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Domain.Entities
{
    public class User : BaseEntity
    {
        /// <summary>البريد الإلكتروني</summary>
        public string Email { get; set; } = string.Empty;        // ✅ Required

        /// <summary>كلمة السر المشفرة</summary>
        public string PasswordHash { get; set; } = string.Empty; // ✅ Required

        /// <summary>الاسم الأول</summary>
        public string FirstName { get; set; } = string.Empty;    // ✅ Required

        /// <summary>الاسم الأخير</summary>
        public string LastName { get; set; } = string.Empty;     // ✅ Required

        /// <summary>رقم الهاتف</summary>
        public string? PhoneNumber { get; set; }                  // ✅ nullable

        /// <summary>صورة الملف الشخصي</summary>
        public string? ProfileImageUrl { get; set; }              // ✅ nullable

        /// <summary>دور المستخدم</summary>
        public UserRole Role { get; set; } = UserRole.Customer;

        /// <summary>هل المستخدم نشط</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>تاريخ آخر دخول</summary>
        public DateTime? LastLoginAt { get; set; }                // ✅ nullable

        /// <summary>العنوان</summary>
        public string? Address { get; set; }                      // ✅ FIX: nullable

        /// <summary>المدينة</summary>
        public string? City { get; set; }                         // ✅ FIX: nullable

        // Navigation Properties
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
