🍽️ Restaurant Real-time Ecosystem (Clean Architecture)

نظام متكامل لإدارة المطاعم مصمم للعمل في بيئات عالية الكثافة، يعتمد على تقنيات .NET 8 الحديثة ويتبع نمط العمارة النظيفة (Clean Architecture) لضمان فصل المسؤوليات وسهولة الصيانة. يتميز النظام بقدرات التواصل اللحظي (Real-time) لربط المطبخ، النادل، والإدارة في شبكة واحدة. 
🛠️ التقنيات المستخدمة (Tech Stack)

    Backend: .NET 8.0 Web API. 

    Real-time Communication: SignalR Hubs (للتنبيهات اللحظية وتحديثات الحالة). 

    Architecture: Clean Architecture (Domain, Application, Infrastructure, WebApi).

    Database: PostgreSQL مع استخدام Entity Framework Core كـ ORM. 

    Security: حماية كاملة باستخدام JWT Authentication و Role-based Authorization (RBAC).

    Mapping & Validation: AutoMapper و FluentValidation.

✨ المميزات الرئيسية (Key Features)

    👨‍🍳 Kitchen Display System (KDS): شاشة لحظية للشيف تستقبل الطلبات فور إنشائها وتسمح بتحديث حالتها إلى "جاهز" بضغطة زر.

    💁‍♂️ Waiter Radar: لوحة تحكم للنادل تستقبل تنبيهات فورية (Push Notifications) عند جاهزية الطلبات للتسليم.

    📊 Admin Live Dashboard: إحصائيات حية للمدير تظهر عدد الطلبات (Pending, Preparing, Ready, Completed) وتتحدث تلقائياً عند أي تغيير في النظام.

    🔐 Secure Order Lifecycle: دورة حياة كاملة للطلب تبدأ من الزبون وتنتهي بالتسليم، مع ضمان سلامة البيانات وحساب المبالغ والضرائب تلقائياً. 

    🤖 AI-Driven Auditing: دمج تقنيات الذكاء الاصطناعي لفحص الكود برمجياً وتقليل الأخطاء في العمليات المالية. 

🏗️ هيكلية المشروع (Project Structure)
Plaintext

src/
├── RestaurantSystem.Domain/         # Entities, Enums, Constants
├── RestaurantSystem.Application/    # Business Logic, DTOs, Interfaces, Hubs
├── RestaurantSystem.Infrastructure/ # Data (DbContext), Repositories, Entity Configurations
└── RestaurantSystem.Api/            # Controllers, Program.cs, Middlewares
🚀 كيفية التشغيل (Setup)

    Clone the Repo: ```bash
    git clone https://github.com/MurtadaHM/RestaurantSystem.git

    Database Configuration: قم بتحديث سلسلة الاتصال (Connection String) في ملف appsettings.json لتربطها بقاعدة بيانات PostgreSQL الخاصة بك. 

    Apply Migrations:
    Bash

    dotnet ef database update

    Run Application: قم بتشغيل المشروع واستخدم Swagger UI لاختبار الـ Endpoints أو افتح شاشات الـ Dashboards الملحقة.
