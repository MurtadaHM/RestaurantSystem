using Microsoft.OpenApi.Models;

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
                    Description = "API لإدارة عمليات المطعم - تطوير مرتضى حسين بواسطة ",
                    Contact = new OpenApiContact
                    {
                        Name = "فريق نظام المطعم",
                        Email = "mmortada721@gmail.com"
                    }
                });

                // ✅ الحل الجذري لمشكلة الـ Error 500 (تشابه الأسماء)
                // يخبر Swagger باستخدام الاسم الكامل للكلاس لتجنب التضارب
                options.CustomSchemaIds(type => type.FullName);

                // إضافة دعم JWT في Swagger
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
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
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

                // خيار اختياري: إذا جعلت RoutePrefix فارغاً، سيفتح Swagger فور تشغيل المشروع (/)
                // بدلاً من (/swagger)
                options.RoutePrefix = string.Empty;

                options.DocumentTitle = "Restaurant Management System";
            });

            return app;
        }
    }
}