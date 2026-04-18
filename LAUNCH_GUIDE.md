# 🚀 دليل الرفع على GitHub و Render (نهائي)

## ✅ التحقق من الجاهزية

```bash
# 1. بناء المشروع
dotnet build -c Release

# 2. اختبار محليّاً مع Docker
docker-compose up -d
# انتظر 10 ثوانٍ
curl http://localhost:5183/swagger/index.html
docker-compose down
```

---

## 📋 قائمة الفحص النهائية (PRE-LAUNCH)

- [x] appsettings.json محدّث بالقيم الصحيحة
- [x] Dockerfile محسّن
- [x] docker-compose.yaml جاهز
- [x] render.yaml موجود
- [x] .env.example موجود
- [x] .env.prod.example موجود
- [x] .gitignore صحيح (يستثني .env)
- [x] GitHub Actions workflow موجود
- [x] البناء بدون أخطاء ✅

---

## 🚀 الخطوة 1️⃣: رفع على GitHub

```bash
# 1. انتقل للمشروع
cd c:\Users\Murtada\source\repos\RestaurantSystem

# 2. تحقق من الملفات المعدّلة
git status

# 3. أضف جميع الملفات
git add .

# 4. اعمل commit
git commit -m "chore: production ready - Docker & Render configured"

# 5. ادفع إلى GitHub
git push origin main
```

**انتظر التنبيه من GitHub أن الـ push نجح ✅**

---

## 🎯 الخطوة 2️⃣: إعداد Render (من الويب)

### أ) إنشاء Web Service

```
1. اذهب إلى https://dashboard.render.com
2. اضغط "New +" الزر الأزرق
3. اختر "Web Service"
4. اختر "Connect a repository"
5. اختر حسابك على GitHub
6. اختر: MurtadaHM/RestaurantSystem
7. اضغط "Connect"
```

### ب) تعديل الإعدادات

```
Name:                    restaurant-api
Environment:             Docker
Repo:                    https://github.com/MurtadaHM/RestaurantSystem
Branch:                  main
Build Command:           (ترك فارغ)
Start Command:           (ترك فارغ)
Docker Context Directory: .
Dockerfile Path:         ./RestaurantSystem.Api/Dockerfile
Region:                  Frankfurt (أو أقرب منطقة)
Plan:                    Free (للتطوير)
```

### ج) إضافة متغيرات البيئة

اضغط "Environment" وأضف هذه المتغيرات:

```env
# Database - اختر A أو B
A) استخدم Render PostgreSQL:
DATABASE_URL=postgresql://user:password@host:5432/database

B) استخدم Database خارجي:
DB_HOST=your_external_host.com
DB_PORT=5432
DB_NAME=restaurant_prod
DB_USER=your_username
DB_PASSWORD=your_very_secure_password_here_32_characters_minimum

# JWT - أنشئ key آمن جديد!
JWT_KEY=your_new_secure_jwt_key_256_bits_recommended_use_openssl_rand

# Claude AI
CLAUDE_API_KEY=sk-ant-your_actual_api_key_here

# Sendy Integration (الفريق الأول)
SENDY_BASE_URL=https://sendy-backend-production.up.railway.app/
SENDY_API_KEY=your_sendy_api_key_here
SENDY_WEBHOOK_SECRET=your_webhook_secret_here

# Team6 Integration (الفريق الثاني)
TEAM6_ENABLED=true
TEAM6_BASE_URL=https://rest-back-qoxj.onrender.com
TEAM6_RESTAURANT_ID=21111111-1111-1111-1111-111111111112
TEAM6_POLLING_INTERVAL=10
TEAM6_FALLBACK_USER_ID=eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee

# Application
LOG_LEVEL=Warning
ASPNETCORE_ENVIRONMENT=Production
```

### د) النشر

```
اضغط الزر الأزرق "Deploy"
شاهد السجلات والانتظر (5-7 دقائق)
عندما يصبح أخضر ✅ "Live" = تم!
```

---

## 🗄️ الخطوة 3️⃣: إنشاء قاعدة بيانات (اختياري)

إذا كنت تريد استخدام قاعدة بيانات مدارة من Render:

```
1. في Render Dashboard
2. اضغط "New +"
3. اختر "PostgreSQL"
4. اسم: restaurant-db
5. انتظر الإنشاء
6. نسخ Connection String
7. أضفه كـ DATABASE_URL في Web Service Environment
```

---

## ⚙️ الخطوة 4️⃣: تطبيق Migrations

بعد النشر الأول مباشرة:

```
1. في Render Dashboard
2. انقر على service الخاصة بك
3. اضغط "Shell" في الأعلى
4. شغّل:

dotnet ef database update \
  --project RestaurantSystem.Infrastructure \
  --startup-project RestaurantSystem.Api

5. انتظر حتى ينتهي (1-2 دقيقة)
```

---

## 🧪 الخطوة 5️⃣: الاختبار

```bash
# بعد النشر، اختبر:

# 1. الـ Swagger UI
curl https://restaurant-api.onrender.com/swagger/index.html

# 2. Health Check
curl https://restaurant-api.onrender.com/health

# 3. API Endpoint
curl https://restaurant-api.onrender.com/api/tables
```

---

## 📊 ملخص بيانات النشر

| المعلومة | القيمة |
|----------|--------|
| **GitHub** | https://github.com/MurtadaHM/RestaurantSystem |
| **Render Service** | restaurant-api |
| **API URL** | https://restaurant-api.onrender.com |
| **Database** | PostgreSQL (Render أو خارجي) |
| **Auto-Deploy** | ✅ يفعّل عند كل push |
| **CI/CD** | ✅ GitHub Actions |

---

## 🔒 نقاط أمان مهمة

⚠️ **تأكد من:**

1. ✅ عدم وجود `.env` في git commit
2. ✅ استخدام كلمات مرور قوية (32+ حرف)
3. ✅ تغيير `JWT_KEY` إلى قيمة فريدة
4. ✅ تفعيل SSL في قاعدة البيانات
5. ✅ عدم نسخ أي API Keys في GitHub

---

## 📞 الأوامر المفيدة

### للتطوير المحلي:

```bash
# تشغيل
docker-compose up -d

# عرض السجلات
docker-compose logs -f

# إيقاف
docker-compose down
```

### لـ GitHub:

```bash
# التحديثات المستقبلية
git add .
git commit -m "chore: update"
git push origin main
# سيفعّل النشر التلقائي على Render!
```

---

## 🎉 النتيجة النهائية

```
┌──────────────────────────────────────┐
│  GitHub Repository                   │
│  https://github.com/MurtadaHM/...    │
│           ↓ (auto-deploy)            │
│  Render Service                      │
│  https://restaurant-api.onrender.com │
│           ↓                          │
│  📱 API متاح على الإنترنت!          │
└──────────────────────────────────────┘
```

---

## ✨ الميزات المفعّلة

✅ **نشر تلقائي** - كل push → نشر فوري  
✅ **CI/CD كامل** - بناء واختبار تلقائي  
✅ **أمان عالي** - جميع secrets في متغيرات بيئة  
✅ **Health Checks** - مراقبة الصحة التلقائية  
✅ **Logging** - سجلات شاملة  
✅ **Multi-Integration** - Sendy + Team6 متكاملتان  

---

## 🆘 إذا حدث خطأ

| المشكلة | الحل |
|--------|------|
| Build Failed | شاهد السجلات في Render Logs |
| DB Connection | تحقق من DATABASE_URL في Environment |
| 502 Error | تطبيق الـ migrations |
| Env Variable | تأكد من اسم المتغير صحيح |

---

## 📖 الملفات المهمة

- `START_HERE.md` - بدايات سريعة
- `RENDER_QUICKSTART.md` - 5 دقائق
- `RENDER_SETUP.md` - شامل 30+ صفحة
- `COMMANDS.md` - أوامر مرجعية

---

## ⏱️ الوقت المتوقع

- رفع على GitHub: **5 دقائق**
- إعداد Render: **10 دقائق**
- النشر الأول: **5-7 دقائق**
- Migrations: **2 دقيقة**
- **الإجمالي: ~30 دقيقة**

---

**آخر تحديث**: 2026-04-18  
**الحالة**: ✅ جاهز 100% للإطلاق!

---

## 🎯 الخطوة التالية

```bash
# 1. رفع على GitHub
git add .
git commit -m "chore: production ready"
git push origin main

# 2. اذهب إلى render.com
# 3. أنشئ Web Service
# 4. أضف Environment Variables
# 5. Deploy!
```

**مبروك! 🎉 نظامك الآن على الإنترنت!**
