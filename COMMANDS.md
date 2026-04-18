# 🛠️ الأوامر المفيدة للتطوير والنشر

اجمعنا هنا أكثر الأوامر الضرورية والمفيدة في مكان واحد.

---

## 🏗️ بناء وتشغيل المشروع

### تشغيل محلي

```bash
# استعادة الـ dependencies
dotnet restore

# بناء المشروع
dotnet build

# تشغيل التطبيق
dotnet run --project RestaurantSystem.Api

# تشغيل مع watch mode (auto-reload)
dotnet watch --project RestaurantSystem.Api
```

### استخدام Docker

```bash
# بناء الصورة
docker build -t restaurant-api:latest -f RestaurantSystem.Api/Dockerfile .

# تشغيل الحاوية
docker run -p 5183:5183 --env-file .env restaurant-api:latest

# تشغيل مع Docker Compose (التطوير)
docker-compose up --build -d

# تشغيل مع Docker Compose (الإنتاج)
docker-compose -f compose.prod.yaml --env-file .env.prod up -d
```

---

## 🗄️ قاعدة البيانات

### التشغيل والـ Migrations

```bash
# تطبيق الـ migrations الجديدة
dotnet ef database update

# إنشاء migration جديد
dotnet ef migrations add MigrationName --project RestaurantSystem.Infrastructure

# إرجاع آخر migration
dotnet ef database update -1

# حذف آخر migration
dotnet ef migrations remove --project RestaurantSystem.Infrastructure
```

### التحقق من حالة قاعدة البيانات

```bash
# الدخول إلى PostgreSQL مباشرة
psql -h localhost -U postgres -d RestaurantDb

# عرض الـ migrations المطبقة
SELECT * FROM __EFMigrationsHistory;

# عرض كل الـ tables
\dt

# خروج من psql
\q
```

---

## 🐳 Docker Commands

### إدارة الصور

```bash
# عرض كل الصور
docker images

# حذف صورة
docker rmi restaurant-api:latest

# بناء صورة بدون cache
docker build --no-cache -t restaurant-api:latest .

# تسميات متعددة لنفس الصورة
docker tag restaurant-api:latest restaurant-api:prod
```

### إدارة الحاويات

```bash
# عرض الحاويات الجارية
docker ps

# عرض كل الحاويات
docker ps -a

# إيقاف حاوية
docker stop restaurant_api

# حذف حاوية
docker rm restaurant_api

# عرض سجلات الحاوية
docker logs -f restaurant_api

# آخر 100 سطر
docker logs --tail 100 restaurant_api

# الدخول إلى الحاوية
docker exec -it restaurant_api bash

# تشغيل أمر في الحاوية
docker exec restaurant_api dotnet --version
```

### Docker Compose

```bash
# تشغيل الخدمات
docker-compose up -d

# إيقاف الخدمات
docker-compose down

# إعادة البناء والتشغيل
docker-compose up --build -d

# عرض السجلات
docker-compose logs -f

# عرض حالة الخدمات
docker-compose ps

# حذف الـ volumes (الحذر!)
docker-compose down -v
```

---

## 🌐 API Testing

### استخدام curl

```bash
# GET request
curl http://localhost:5183/api/tables

# GET مع authentication
curl -H "Authorization: Bearer YOUR_TOKEN" http://localhost:5183/api/orders

# POST request
curl -X POST http://localhost:5183/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"user@example.com","password":"password"}'

# HEAD request
curl -I http://localhost:5183/swagger

# DELETE request
curl -X DELETE http://localhost:5183/api/tables/1 \
  -H "Authorization: Bearer YOUR_TOKEN"
```

### استخدام httpie (أسهل من curl)

```bash
# تثبيت httpie
pip install httpie

# GET request
http GET http://localhost:5183/api/tables

# POST request
http POST http://localhost:5183/api/auth/login \
  email=user@example.com password=password
```

### Swagger UI

```
http://localhost:5183/swagger/index.html
```

---

## 🔐 متغيرات البيئة

### إنشاء .env من .env.example

```bash
# Linux/Mac
cp .env.example .env

# Windows PowerShell
Copy-Item .env.example -Destination .env
```

### إنشاء JWT Key آمن

```bash
# Linux/Mac (openssl)
openssl rand -base64 32

# Windows PowerShell
[Convert]::ToBase64String((1..32 | ForEach-Object { [byte](Get-Random -Maximum 256) }))

# Python
python3 -c "import secrets; print(secrets.token_urlsafe(32))"
```

### عرض المتغيرات

```bash
# عرض كل متغيرات البيئة
env | grep DB_

# في Docker
docker exec restaurant_api env | grep JWT_
```

---

## 📋 Git Commands

### الإعدادات الأساسية

```bash
# تعيين الـ global user
git config --global user.name "Your Name"
git config --global user.email "your.email@example.com"

# تعيين الـ user لـ repository محدد
git config user.name "Your Name"
git config user.email "your.email@example.com"
```

### الـ Workflow الأساسي

```bash
# عرض حالة الملفات
git status

# إضافة الملفات
git add .

# إلغاء التتبع
git reset HEAD filename

# إنشاء commit
git commit -m "feat: add new feature"

# إرسال التغييرات
git push origin main

# جلب التغييرات
git pull origin main
```

### الفروع

```bash
# إنشاء فرع جديد
git checkout -b feature/my-feature

# الانتقال بين الفروع
git checkout main

# عرض الفروع المحلية
git branch

# عرض كل الفروع
git branch -a

# حذف فرع
git branch -d feature/my-feature

# دمج فرع
git merge feature/my-feature
```

### السجل

```bash
# عرض السجل
git log

# عرض آخر 5 commits
git log -5

# عرض بتنسيق مرتب
git log --oneline --graph --all
```

---

## 🧹 التنظيف والصيانة

### تنظيف Docker

```bash
# حذف الحاويات غير المستخدمة
docker container prune

# حذف الصور غير المستخدمة
docker image prune

# حذف الـ volumes غير المستخدمة
docker volume prune

# حذف كل الموارد غير المستخدمة
docker system prune -a
```

### تنظيف المشروع

```bash
# حذف الـ bin و obj
dotnet clean

# حذف git ignored files
git clean -fd

# إعادة تعيين السجل الـ local
git reset --hard origin/main
```

---

## 📊 المراقبة والتشخيص

### معلومات النظام

```bash
# استهلاك الموارد
docker stats

# معلومات Docker
docker version

# معلومات الصورة
docker inspect restaurant-api:latest

# معلومات الحاوية
docker inspect restaurant_api
```

### الـ Logs المتقدمة

```bash
# آخر 1000 سطر
docker logs --tail 1000 restaurant_api

# مع timestamps
docker logs --timestamps restaurant_api

# منذ 10 دقائق
docker logs --since 10m restaurant_api

# في real-time
docker logs -f restaurant_api

# مع grep
docker logs restaurant_api | grep ERROR
```

---

## 🚀 أوامر النشر

### Render

```bash
# بناء صورة Render
docker build --target final \
  --tag ghcr.io/murtadahm/restaurantsystem:latest .

# اختبار الصورة محلياً قبل الـ push
docker run -p 5183:5183 \
  --env-file .env.prod \
  ghcr.io/murtadahm/restaurantsystem:latest

# إرسال الـ push إلى GitHub Packages
docker push ghcr.io/murtadahm/restaurantsystem:latest
```

### الـ Commit والـ Push

```bash
# commit نهائي قبل النشر
git add .
git commit -m "chore: ready for production deployment"

# push إلى main (سيؤدي لـ auto-deploy على Render)
git push origin main
```

---

## 🆘 استكشاف الأخطاء

### مشاكل البناء

```bash
# بناء مفصل مع verbose
dotnet build -v detailed

# تنظيف كامل ثم بناء
dotnet clean && dotnet build

# بناء بدون dependencies
dotnet build --no-restore
```

### مشاكل التشغيل

```bash
# تشغيل مع verbose logging
ASPNETCORE_ENVIRONMENT=Development dotnet run

# مع console logging
dotnet run --verbosity:diagnostic

# من داخل Docker
docker logs -f restaurant_api
```

### مشاكل قاعدة البيانات

```bash
# اختبار الاتصال
psql -h localhost -U postgres -d RestaurantDb -c "SELECT 1;"

# من داخل Docker
docker exec -it restaurant_postgres \
  psql -U postgres -d RestaurantDb -c "SELECT 1;"

# تفريغ البيانات
pg_dump -U postgres -d RestaurantDb > backup.sql
```

---

## 💡 نصائح مفيدة

### اختصارات مفيدة في PowerShell

```powershell
# تشغيل docker-compose بسهولة
Set-Alias dc docker-compose

# اختصار لـ git
Set-Alias g git

# استخدام الاختصارات
g push
dc up -d
```

### اختصارات bash/zsh

```bash
# أضف لـ ~/.bashrc أو ~/.zshrc
alias dc='docker-compose'
alias g='git'
alias dotnet-watch='dotnet watch run'

# source الملف
source ~/.bashrc
```

---

## 📚 مصادر إضافية

| الموضوع | الأمر | الملف |
|---------|--------|-------|
| البدء السريع | - | [RENDER_QUICKSTART.md](RENDER_QUICKSTART.md) |
| إعداد Render | - | [RENDER_SETUP.md](RENDER_SETUP.md) |
| نشر عام | - | [DEPLOYMENT.md](DEPLOYMENT.md) |
| قائمة التحقق | - | [PRODUCTION_CHECKLIST.md](PRODUCTION_CHECKLIST.md) |

---

**آخر تحديث**: 2026-04-18  
**الإصدار**: 1.0.0
