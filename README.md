🍽️ Restaurant Real-time Ecosystem (Clean Architecture)

نظام متكامل لإدارة المطاعم مصمم للعمل في بيئات عالية الكثافة، يعتمد على تقنيات .NET 8 الحديثة ويتبع نمط العمارة النظيفة (Clean Architecture) لضمان فصل المسؤوليات وسهولة الصيانة. يتميز النظام بقدرات التواصل اللحظي (Real-time) لربط المطبخ، النادل، والإدارة في شبكة واحدة. 

---

## 🛠️ التقنيات المستخدمة (Tech Stack)

- **Backend**: .NET 8.0 Web API
- **Real-time Communication**: SignalR Hubs (للتنبيهات اللحظية وتحديثات الحالة)
- **Architecture**: Clean Architecture (Domain, Application, Infrastructure, WebApi)
- **Database**: PostgreSQL مع استخدام Entity Framework Core كـ ORM
- **Security**: حماية كاملة باستخدام JWT Authentication و Role-based Authorization (RBAC)
- **Mapping & Validation**: AutoMapper و FluentValidation
- **Containerization**: Docker و Docker Compose

---

## ✨ المميزات الرئيسية (Key Features)

- **👨‍🍳 Kitchen Display System (KDS)**: شاشة لحظية للشيف تستقبل الطلبات فور إنشائها وتسمح بتحديث حالتها إلى "جاهز"
- **💁‍♂️ Waiter Radar**: لوحة تحكم للنادل تستقبل تنبيهات فورية عند جاهزية الطلبات
- **📊 Admin Live Dashboard**: إحصائيات حية للمدير مع تحديثات فورية
- **🔐 Secure Order Lifecycle**: دورة حياة آمنة للطلب مع حساب تلقائي للضرائب
- **🤖 AI-Driven Auditing**: دمج تقنيات الذكاء الاصطناعي للفحص الآلي

---

## 🏗️ هيكلية المشروع (Project Structure)

```
RestaurantSystem/
├── RestaurantSystem.Domain/         # Entities, Enums, Constants
├── RestaurantSystem.Application/    # Business Logic, DTOs, Interfaces
├── RestaurantSystem.Infrastructure/ # Data, Repositories, Configurations
├── RestaurantSystem.Api/            # Controllers, Middlewares, SignalR Hubs
├── Dockerfile                       # صورة Docker للـ API
├── compose.yaml                     # Docker Compose للتطوير
├── compose.prod.yaml                # Docker Compose للإنتاج
└── DEPLOYMENT.md                    # دليل النشر الشامل
```

---

## 🚀 كيفية البدء السريع

### المتطلبات
- .NET 8 SDK
- PostgreSQL 12+
- Docker و Docker Compose (اختياري)

### التشغيل المحلي

```bash
# استنساخ المستودع
git clone https://github.com/MurtadaHM/RestaurantSystem.git
cd RestaurantSystem

# نسخ ملف الإعدادات
cp .env.example .env

# تطبيق الـ Migrations
dotnet ef database update --project RestaurantSystem.Infrastructure

# تشغيل التطبيق
dotnet run --project RestaurantSystem.Api
```

الـ API متاح على: **http://localhost:5183**
Swagger UI: **http://localhost:5183/swagger/index.html**

### التشغيل باستخدام Docker

```bash
# نسخ ملف الإعدادات
cp .env.example .env

# البناء والتشغيل
docker-compose up -d

# عرض السجلات
docker-compose logs -f
```

---

## 📦 النشر على السيرفر الخارجي

### ✅ نقاط مهمة قبل النشر

1. **الـ Secrets آمنة**: تم نقل جميع المفاتيح السرية إلى متغيرات البيئة
2. **ملفات التكوين**: استخدام `.env` و `.env.prod` لإدارة الإعدادات
3. **Docker Ready**: الـ Dockerfile وملفات Compose جاهزة للإنتاج

### خطوات النشر

#### الطريقة 1: باستخدام Script النشر (موصى به)

```bash
# جعل السكريبت قابلاً للتنفيذ
chmod +x deploy.sh

# تشغيل السكريبت
./deploy.sh
```

#### الطريقة 2: يدويًا

```bash
# 1. نسخ الملفات إلى السيرفر
scp -r . user@server.com:/app/restaurant

# 2. الدخول إلى السيرفر
ssh user@server.com

# 3. تحضير البيئة
cd /app/restaurant
cp .env.prod.example .env.prod

# تعديل ملف .env.prod بقيمك الفعلية
nano .env.prod

# 4. بناء الصورة والتشغيل
docker-compose -f compose.prod.yaml --env-file .env.prod build
docker-compose -f compose.prod.yaml --env-file .env.prod up -d

# 5. التحقق من الحالة
docker ps
docker logs restaurant_api_prod
```

### قائمة التحقق من النشر

- [ ] تم نسخ `.env.prod.example` إلى `.env.prod`
- [ ] تم تحديث جميع متغيرات البيئة بقيم الإنتاج
- [ ] تم التحقق من اتصال قاعدة البيانات
- [ ] تم اختبار الـ API endpoints
- [ ] تم إعداد النسخ الاحتياطية
- [ ] تم إعداد المراقبة والتنبيهات

---

## 📚 التوثيق الإضافية

- **[دليل النشر الشامل](DEPLOYMENT.md)** - تعليمات مفصلة للنشر والمراقبة
- **API Documentation** - متاح على `/swagger` بعد التشغيل

---

## 🔒 الأمان

### Best Practices المتبعة

✅ استخدام JWT للتوثيق
✅ RBAC لإدارة الصلاحيات
✅ فصل الـ Secrets عن الكود
✅ استخدام متغيرات البيئة للإعدادات الحساسة
✅ CORS محدود للإنتاج
✅ Health checks مدمجة

---

## 🐛 استكشاف الأخطاء

### الخطأ: "Cannot connect to database"
```bash
# تحقق من متغيرات البيئة
docker exec restaurant_api env | grep DB_

# اختبر الاتصال
docker exec restaurant_api psql -h $DB_HOST -U $DB_USER -d $DB_NAME -c "SELECT 1;"
```

### الخطأ: "Port already in use"
```bash
# غير المنفذ في .env
sed -i 's/API_PORT=5183/API_PORT=5184/' .env

# أعد التشغيل
docker-compose restart
```

---

## 📊 المراقبة

```bash
# عرض استخدام الموارد
docker stats restaurant_api

# عرض السجلات الحالية
docker logs --tail 100 -f restaurant_api

# إحصائيات النظام
docker system df
```

---

## 🤝 المساهمة

نرحب بالمساهمات! يرجى:

1. Fork المستودع
2. إنشاء فرع للميزة الجديدة (`git checkout -b feature/amazing-feature`)
3. Commit التغييرات (`git commit -m 'Add amazing feature'`)
4. Push للفرع (`git push origin feature/amazing-feature`)
5. فتح Pull Request

---

## 📝 الترخيص

هذا المشروع مرخص تحت [MIT License](LICENSE)

---

## 📧 التواصل

- **المؤلف**: Murtada HM
- **البريد الإلكتروني**: [murtadahm@example.com]
- **GitHub**: [MurtadaHM](https://github.com/MurtadaHM)

---

**آخر تحديث**: 2026-04-18 | **الإصدار**: 1.0.0
