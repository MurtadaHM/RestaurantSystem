using Microsoft.OpenApi.Models;
using System.Reflection;

namespace RestaurantSystem.Api.Configurations
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Restaurant Management System API",
                    Version = "v1",
                    Description = "API متكامل لإدارة عمليات المطعم (الطلبات، الطاولات، الأقسام) - تطوير المبرمج مرتضى حسين",
                    Contact = new OpenApiContact
                    {
                        Name = "تواصل مع الادارة",
                        Email = "mmortada721@gmail.com"
                    }
                });

                // ✅ 1. إظهار التعليقات (Summary) في واجهة Swagger
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }

                // ✅ 2. الحل الذكي لتضارب الأسماء (Schema IDs)
                // بدلاً من FullName الممل، نستخدم الاسم فقط إلا في حال وجود تضارب
                options.CustomSchemaIds(type => type.ToString().Replace("RestaurantSystem.Application.DTOs.", ""));

                // 🔐 3. إعدادات الحماية (JWT)
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "ادخل الـ Token بهذا الشكل: Bearer {your_token}"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant API v1");
                options.RoutePrefix = string.Empty; // يفتح Swagger مباشرة عند التشغيل
                options.DocumentTitle = "Restaurant System Documentation";

                // ✅ جعل واجهة الـ Schemas مغلقة افتراضياً ليكون الشكل أرشق
                options.DefaultModelsExpandDepth(-1);
            });

            return app;
        }
    }
}