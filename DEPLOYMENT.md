# 🚀 دليل نشر Restaurant System على سيرفر خارجي

## ✅ المتطلبات قبل البدء

- **Docker** و **Docker Compose** مثبتة على السيرفر
- **Git** مثبت (اختياري)
- خادم **PostgreSQL** خارجي أو محلي
- الملفات الحساسة (.env) آمنة وغير مشاركة في Git

---

## 📋 خطوات النشر

### 1️⃣ نسخ الملفات إلى السيرفر

```bash
# استخدام Git
git clone https://github.com/YOUR_USERNAME/RestaurantSystem.git
cd RestaurantSystem

# أو استخدام SCP/SFTP
scp -r . user@server.com:/path/to/app/
```

### 2️⃣ تحضير ملفات البيئة

```bash
# للتطوير (مع قاعدة بيانات محلية)
cp .env.example .env
# عدّل الملف بقيم التطوير
nano .env

# للإنتاج (مع قاعدة بيانات خارجية)
cp .env.prod.example .env.prod
# عدّل الملف بقيم الإنتاج الفعلية
nano .env.prod
```

### 3️⃣ بناء صورة Docker

```bash
# للتطوير
docker-compose build

# للإنتاج
docker-compose -f compose.prod.yaml build
```

### 4️⃣ تشغيل التطبيق

#### للبيئة التطويرية (مع PostgreSQL):
```bash
docker-compose up -d
```

#### للبيئة الإنتاجية (مع قاعدة بيانات خارجية):
```bash
docker-compose -f compose.prod.yaml --env-file .env.prod up -d
```

### 5️⃣ التحقق من الحالة

```bash
# عرض السجلات
docker-compose logs -f

# أو للإنتاج
docker-compose -f compose.prod.yaml logs -f

# التحقق من حالة الحاويات
docker ps

# اختبار الـ API
curl http://localhost:5183/swagger/index.html
```

### 6️⃣ تطبيق الـ Migrations (اختياري - إذا كانت قاعدة البيانات جديدة)

```bash
# الدخول إلى حاوية التطبيق
docker exec -it restaurant_api bash

# تطبيق الـ migrations
dotnet ef database update --project RestaurantSystem.Infrastructure --startup-project RestaurantSystem.Api
```

---

## 🔒 أفضل الممارسات الأمنية

### 1. حماية ملفات .env

```bash
# جعل الملف قابل للقراءة للمالك فقط
chmod 600 .env
chmod 600 .env.prod

# إضافة إلى .gitignore (إذا لم يكن موجوداً)
echo ".env" >> .gitignore
echo ".env.prod" >> .gitignore
```

### 2. استخدام متغيرات بيئة قوية

```bash
# توليد JWT Key آمن
openssl rand -base64 32

# أو استخدام Python
python3 -c "import secrets; print(secrets.token_urlsafe(32))"
```

### 3. تفعيل HTTPS

```yaml
# استخدام Nginx Reverse Proxy (اختياري)
# أضف قسم nginx إلى compose.prod.yaml
nginx:
  image: nginx:alpine
  ports:
    - "80:80"
    - "443:443"
  volumes:
    - ./nginx.conf:/etc/nginx/nginx.conf:ro
    - ./certs:/etc/nginx/certs:ro
  depends_on:
    - restaurantsystemapi
```

---

## 📊 مراقبة الأداء

### عرض استخدام الموارد

```bash
docker stats restaurant_api
docker stats restaurant_postgres
```

### الحصول على معلومات التطبيق

```bash
# عدد السجلات
docker logs restaurant_api | wc -l

# آخر 100 سطر
docker logs --tail 100 restaurant_api

# بتوقيت حقيقي
docker logs -f restaurant_api
```

---

## 🔄 إعادة التشغيل والتحديث

### إيقاف التطبيق

```bash
docker-compose down
# أو للإنتاج
docker-compose -f compose.prod.yaml down
```

### تحديث الملف البرمجي

```bash
git pull origin main

# إعادة البناء والتشغيل
docker-compose up --build -d
```

### استعادة البيانات بعد الإيقاف

```bash
# البيانات في PostgreSQL آمنة إذا كانت في volume
docker volume ls

# عرض volumes المستخدمة
docker inspect restaurant_postgres
```

---

## 🐛 استكشاف الأخطاء

### الخطأ: "Cannot connect to database"

```bash
# تحقق من متغيرات البيئة
docker exec restaurant_api env | grep DB_

# تجربة الاتصال من داخل الحاوية
docker exec restaurant_api psql -h $DB_HOST -U $DB_USER -d $DB_NAME -c "SELECT 1;"
```

### الخطأ: "API not responding"

```bash
# تحقق من السجلات
docker logs restaurant_api

# تحقق من المنافذ المستخدمة
netstat -tuln | grep 5183

# أعد تشغيل الحاوية
docker restart restaurant_api
```

### الخطأ: "Disk space full"

```bash
# عرض مساحة القرص
docker system df

# حذف الصور والحاويات غير المستخدمة
docker system prune -a
```

---

## 📈 التوسع والنمو

### استخدام Docker Swarm (للتطبيقات الكبيرة)

```bash
# تهيئة Swarm
docker swarm init

# نشر الخدمة
docker stack deploy -c compose.yaml restaurant_system
```

### استخدام Kubernetes (للتطبيقات الموزعة)

```bash
# تحويل docker-compose إلى Kubernetes manifests
kompose convert -f compose.prod.yaml -o kubernetes/
```

---

## 🆘 الدعم والمساعدة

- **التوثيق**: [Docker Docs](https://docs.docker.com/)
- **المجتمع**: [Docker Community](https://www.docker.com/community)
- **الإصدار الجديد**: راجع [GitHub Releases](https://github.com/YOUR_USERNAME/RestaurantSystem/releases)

---

## ✨ نصائح إضافية

1. **استخدم `.dockerignore`**: تم تحديثه ليستثني الملفات غير الضرورية
2. **نسخ احتياطية دورية**: احرص على نسخ احتياطية من PostgreSQL
3. **الرصد المستمر**: استخدم أدوات مثل Prometheus و Grafana
4. **التنبيهات**: قم بإعداد التنبيهات لحالات الأخطاء الحرجة

---

**آخر تحديث**: 2026-04-18 | **الإصدار**: 1.0.0
