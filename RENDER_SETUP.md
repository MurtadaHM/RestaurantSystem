# 🚀 دليل النشر على Render

## ✨ مميزات Render

✅ نشر تلقائي من GitHub  
✅ دعم كامل لـ Docker  
✅ PostgreSQL مدمجة  
✅ متغيرات بيئة آمنة  
✅ SSL/HTTPS تلقائي  
✅ Monitoring و Logs  

---

## 📋 المتطلبات

- حساب على [render.com](https://render.com)
- Repository على GitHub مع المشروع
- الملفات التالية موجودة في المشروع:
  - ✅ `Dockerfile`
  - ✅ `render.yaml` (اختياري لكن موصى به)
  - ✅ `.env.prod.example`

---

## 🔧 خطوات الإعداد

### الخطوة 1️⃣: إنشاء حساب Render وربط GitHub

```
1. اذهب إلى https://render.com
2. سجل حساب جديد أو تسجيل الدخول
3. اتصل GitHub بـ Render:
   - اذهب إلى Settings → Connected Services
   - اضغط Connect GitHub
   - صرح بالوصول للمستودع
```

### الخطوة 2️⃣: إنشاء خدمة Web جديدة

#### الطريقة الأولى: من لوحة التحكم (سهلة)

```
1. اضغط "New +" → "Web Service"
2. اختر مستودعك (RestaurantSystem)
3. اختر الإعدادات التالية:
   - Name: restaurant-api (أو أي اسم تفضله)
   - Region: Frankfurt (قرب القارة الأوروبية - أسرع)
   - Branch: main
   - Runtime: Docker
```

#### الطريقة الثانية: باستخدام render.yaml

```
1. اضغط "New +" → "Web Service"
2. اختر المستودع
3. سيكتشف render.yaml تلقائياً
4. اتابع الخطوات التالية
```

---

## 🔐 خطوة 3: تعيين متغيرات البيئة

في لوحة تحكم Render، اذهب إلى **Environment** وأضف المتغيرات التالية:

### Database Variables
```env
DB_USER=postgres
DB_PASSWORD=your_secure_password_here_32_characters_min
DATABASE_URL=postgres://user:password@host:5432/restaurant_prod
```

### JWT Configuration
```env
JWT_KEY=your_very_secure_jwt_key_here_256_bits_minimum_recommended_use_openssl_rand_base64_32
JWT_ISSUER=RestaurantSystem
JWT_AUDIENCE=RestaurantSystemUsers
JWT_EXPIRY_MINUTES=1440
```

### External Services
```env
CLAUDE_API_KEY=sk-ant-...your_claude_api_key_here
SENDY_BASE_URL=https://sendy-backend-production.up.railway.app/
SENDY_API_KEY=your_sendy_api_key
SENDY_WEBHOOK_SECRET=your_webhook_secret

TEAM6_BASE_URL=https://rest-back-qoxj.onrender.com
TEAM6_RESTAURANT_ID=21111111-1111-1111-1111-111111111112
TEAM6_FALLBACK_USER_ID=eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee
```

### Application Configuration
```env
LOG_LEVEL=Warning
ASPNETCORE_ENVIRONMENT=Production
```

---

## 🗄️ الخطوة 4: إنشاء PostgreSQL Database

### الخيار 1: استخدام Render PostgreSQL (موصى به)

```
1. في لوحة Render، اضغط "New +" → "PostgreSQL"
2. الإعدادات:
   - Name: restaurant-db
   - PostgreSQL Version: 15 (أحدث إصدار مستقر)
   - Region: Frankfurt (نفس منطقة API)
3. نسخ Connection String وأضفه كـ DATABASE_URL
```

### الخيار 2: استخدام خادم خارجي

إذا كان لديك خادم PostgreSQL خارجي:

```
1. تأكد من الاتصال الآمن (SSL)
2. أضف الـ connection string إلى DATABASE_URL
3. تأكد من أن الـ firewall يسمح بـ Render IPs
```

---

## ▶️ الخطوة 5: النشر الأول

### الطريقة اليدوية:

```
1. في لوحة الخدمة، اضغط "Deploy" (الزر الأزرق)
2. شاهد السجلات في Logs:
   ```
   Cloning repository...
   Building Docker image...
   Starting service...
   ```
3. انتظر حتى تظهر الرسالة:
   ```
   ✓ Service is live at: https://restaurant-api.onrender.com
   ```
```

### النشر التلقائي:

```
بمجرد أن تدفع (push) تغييرات إلى main، سيتم:
1. كشف التغييرات تلقائياً
2. بناء صورة Docker جديدة
3. نشرها تلقائياً (إذا كان autoDeploy: true)
```

---

## ✅ اختبار التطبيق

```bash
# بعد النشر، اختبر الـ API

# الـ Swagger UI
curl https://restaurant-api.onrender.com/swagger/index.html

# Health Check
curl https://restaurant-api.onrender.com/health

# اختبر Endpoint معين
curl -X GET https://restaurant-api.onrender.com/api/tables
```

---

## 🔄 تطبيق Database Migrations

### بعد النشر الأول:

```bash
# 1. الدخول إلى محطة Render (Terminal)
#    في لوحة التحكم → Shell

# 2. تشغيل الـ migrations
dotnet ef database update \
  --project RestaurantSystem.Infrastructure \
  --startup-project RestaurantSystem.Api

# 3. تحقق من النتائج
```

---

## 📊 مراقبة التطبيق

### السجلات (Logs)

```
في لوحة الخدمة:
- اضغط على "Logs" tab
- شاهد السجلات الحالية
- فلتر حسب المستوى (Error, Warning, Info)
```

### المراقبة المتقدمة

```
في Settings:
- Event Notifications: فعّل التنبيهات عند الأخطاء
- Metrics: شاهد استهلاك الموارد
```

---

## 🐛 استكشاف الأخطاء الشائعة

### الخطأ: "Build Failed"

```bash
# 1. تحقق من الـ Dockerfile
docker build -t test .

# 2. تحقق من المتغيرات المفقودة
grep "\${" render.yaml

# 3. شاهد السجلات الكاملة في Render
```

### الخطأ: "Database Connection Failed"

```
1. تحقق من DATABASE_URL في Environment
2. تأكد من أن DB_USER و DB_PASSWORD صحيحة
3. تحقق من أن قاعدة البيانات في نفس المنطقة
4. شغّل:
   dotnet ef database update
```

### الخطأ: "Unhealthy Service"

```
1. اضغط على Service → Metrics
2. شاهد الـ CPU و Memory
3. راجع السجلات للأخطاء
4. قد تحتاج لـ restart:
   - Deploy → Clear Build Cache → Deploy
```

### المنفذ غير صحيح

```
تأكد من:
1. Dockerfile يعرّض PORT 5183
2. render.yaml يحدد port: 5183
3. البيئة لا تحتاج لـ HTTPS redirect
```

---

## 🔄 عملية التحديث

### للتحديثات البسيطة:

```bash
# 1. اعمل التغييرات محلياً
git add .
git commit -m "Fix: update configuration"
git push origin main

# 2. Render سينشر تلقائياً في خلال دقيقة
```

### لتحديثات قاعدة البيانات:

```bash
# 1. أضف Migration جديد محلياً
dotnet ef migrations add MigrationName \
  --project RestaurantSystem.Infrastructure

# 2. Push التغييرات
git push origin main

# 3. في Render، اضغط Shell وشغّل:
dotnet ef database update
```

---

## 🔒 أفضل الممارسات الأمنية

### 1. الـ Secrets
```
✅ DO:
- استخدم Environment Variables في Render
- لا تضع secrets في الكود

❌ DON'T:
- لا تعرّض API keys في README
- لا تضع passwords في environment files
```

### 2. Database
```
✅ DO:
- استخدم SSL للاتصال
- فعّل backups تلقائية
- استخدم كلمات مرور قوية

❌ DON'T:
- لا تسمح بـ public access لقاعدة البيانات
- لا تستخدم نفس password في كل مكان
```

### 3. Deployments
```
✅ DO:
- استخدم Pull Requests للمراجعة
- اختبر locally قبل الـ push
- احتفظ بـ backups قبل التحديثات

❌ DON'T:
- لا تنشر directly من main بدون اختبار
- لا تحذف الـ database بدون backup
```

---

## 📈 التوسع المستقبلي

### إذا احتاج التطبيق لـ scaling:

```
1. زيادة عدد الـ instances:
   - Settings → Scaling → Num Instances

2. استخدام Redis للـ caching:
   - New + → Redis
   - ربطه بـ المتغيرات

3. استخدام CDN:
   - Render integrates مع Cloudflare
```

---

## 📞 الدعم والموارد

- **Render Documentation**: https://render.com/docs
- **GitHub Integration**: https://render.com/docs/github
- **Docker Support**: https://render.com/docs/docker
- **Database**: https://render.com/docs/databases

---

## ✨ ملخص الخطوات السريعة

```bash
# 1. تأكد من أن كل شيء مضبوط محلياً
docker-compose -f compose.prod.yaml build

# 2. اضغط التغييرات إلى GitHub
git add .
git commit -m "chore: ready for Render deployment"
git push origin main

# 3. في Render:
#    - ربط GitHub
#    - اختر المستودع
#    - أضف Environment Variables
#    - اضغط Deploy

# 4. بعد النشر:
#    - تطبيق الـ migrations
#    - اختبار الـ API
#    - إعداد المراقبة
```

---

## 🎉 مبروك!

تطبيقك الآن مرفوع على **Render** ويعمل بـ **Docker**! 🚀

**Your API is live at**: `https://restaurant-api.onrender.com`

---

**آخر تحديث**: 2026-04-18 | **الإصدار**: 1.0.0
