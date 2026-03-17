using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RestaurantSystem.Application;
using RestaurantSystem.Infrastructure;
using RestaurantSystem.Infrastructure.Data;
using RestaurantSystem.Api.Filters;
using RestaurantSystem.Api.Middlewares;
using RestaurantSystem.Api.Configurations; // ✅ مهم جداً لاستدعاء SwaggerExtensions
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. إعداد الـ Controllers والفلتر العالمي
// ==========================================
builder.Services.AddControllers(options =>
{
    // 1. إضافة الفلتر الخاص بك
    options.Filters.Add<ValidationFilter>();
})
// 2. ✅ إضافة المحول السحري للنصوص (Enums as Strings)
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
})
// 3. إعدادات سلوك الـ API
.ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// ==========================================
// 2. إعداد Swagger (استخدام الـ Extension Method)
// ==========================================
// تم نقل كل كود الـ AddSwaggerGen المعقد إلى ملف SwaggerExtensions
builder.Services.AddSwaggerDocumentation();

// ==========================================
// 3. تسجيل طبقات المشروع (Clean Architecture)
// ==========================================
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// ✅ تحديث AutoMapper (بعد حذف الحزمة الإضافية القديمة)
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// ==========================================
// 4. إعدادات الحماية (Authentication & JWT)
// ==========================================
var jwtKey = builder.Configuration["Jwt:Key"]
             ?? throw new InvalidOperationException("خطأ: مفتاح JWT غير موجود في appsettings.json");
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtKey)),
        ValidateIssuer = !string.IsNullOrEmpty(jwtIssuer),
        ValidIssuer = jwtIssuer,
        ValidateAudience = !string.IsNullOrEmpty(jwtAudience),
        ValidAudience = jwtAudience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// ==========================================
// 5. إعداد CORS
// ==========================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// ==========================================
// 6. تفعيل الـ Middlewares بالترتيب الصحيح
// ==========================================

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<LoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    // ✅ استخدام الـ Extension Method التي أعددناها
    // ستقوم بفتح Swagger على المسار الرئيسي (/) وبدون أخطاء 500
    app.UseSwaggerDocumentation();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ==========================================
// 7. تفعيل الـ Seed Data تلقائياً
// ==========================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        await DbInitializer.SeedAsync(context);
        Console.WriteLine("---> Database Seeding Completed Successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"---> Error during seeding: {ex.Message}");
    }
}

app.Run();