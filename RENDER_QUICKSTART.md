# 🎯 Quick Start: Render Deployment

اتبع هذه الخطوات لنشر المشروع على Render في أقل من 10 دقائق.

---

## ⚡ الخطوات السريعة

### 1. تحضير GitHub

```bash
# تأكد من وجود آخر التعديلات
git add .
git commit -m "chore: ready for Render"
git push origin main
```

### 2. إعداد Render (من الويب)

```
1. اذهب إلى https://render.com
2. اضغط "New Web Service"
3. اختر "Connect a repository" → اختر RestaurantSystem
4. اضغط "Create Web Service"
```

### 3. تعيين الإعدادات الأساسية

في صفحة الخدمة الجديدة:

```
Name:                    restaurant-api
Environment:             Docker
Build Command:           (ترك فارغ - سيستخدم Dockerfile)
Start Command:           (ترك فارغ)
Docker Context Directory: . (نقطة)
Dockerfile Path:         ./RestaurantSystem.Api/Dockerfile
Region:                  Frankfurt (أو أقرب منطقة)
Plan:                    Free (للبداية) أو Starter
```

### 4. إضافة متغيرات البيئة

اضغط "Environment" وأضف:

#### الحد الأدنى (Required):

```env
DB_HOST=localhost
DB_PORT=5432
DB_NAME=restaurant_prod
DB_USER=postgres
DB_PASSWORD=change_me_securely_32_chars_min

JWT_KEY=your_very_secure_key_here_256_bits
ASPNETCORE_ENVIRONMENT=Production
LOG_LEVEL=Warning
```

#### الاختياري (API Keys):

```env
CLAUDE_API_KEY=your_key_here
SENDY_BASE_URL=your_url
SENDY_API_KEY=your_key_here
SENDY_WEBHOOK_SECRET=your_secret
```

### 5. إعداد قاعدة البيانات

#### الخيار A: استخدام Render PostgreSQL (سهل)

```
1. اضغط "New +" في dashboard
2. اختر "PostgreSQL"
3. انتظر الإنشاء (دقيقتان)
4. نسخ Connection String
5. أضفه كـ DATABASE_URL في Environment Variables
```

#### الخيار B: استخدام Database خارجية

```
فقط أضف DATABASE_URL في Environment
```

### 6. نشر التطبيق

```
1. اضغط الزر الأزرق "Deploy"
2. شاهد السجلات في Logs tab
3. انتظر حتى يصبح أخضر ✓ "Live"
```

### 7. تطبيق Migrations

بعد النشر الأول:

```
1. اضغط "Shell" في Dashboard
2. شغّل:

dotnet ef database update \
  --project RestaurantSystem.Infrastructure \
  --startup-project RestaurantSystem.Api
```

### 8. اختبر الخدمة

```bash
# استبدل restaurant-api باسم خدمتك
curl https://restaurant-api.onrender.com/swagger/index.html
```

---

## ⚙️ التكوين المتقدم (اختياري)

### استخدام render.yaml (توصيتنا)

الملف موجود بالفعل. يمكنك ترك Render يكتشفه تلقائياً.

### GitHub Actions (CI/CD)

الـ workflow موجود في `.github/workflows/deploy.yml`

```
كل push إلى main:
✓ بناء المشروع
✓ اختبار الكود
✓ بناء صورة Docker
✓ scan للأمان
✓ إرسال تنبيه
```

---

## 🆘 المشاكل الشائعة

| المشكلة | الحل |
|--------|------|
| Build Failed | شاهد السجلات، تحقق من Dockerfile path |
| Database not found | أضف DATABASE_URL في Environment |
| 502 Bad Gateway | اختبر API محلياً، تحقق من logs |
| Unhealthy Service | اضغط "Clear Build Cache" ثم "Deploy" |

---

## 📞 الدعم السريع

- Render Docs: https://render.com/docs
- Status Page: https://status.render.com/
- Community: https://render.com/community

---

## ✅ Checklist

- [ ] GitHub repository مرفوع
- [ ] Render account يعمل
- [ ] GitHub متصل بـ Render
- [ ] Environment Variables تم إضافتها
- [ ] Database جاهزة
- [ ] Deployment ناجح (أخضر)
- [ ] Migrations طُبقت
- [ ] API تم اختباره

---

## 🎉 تمام!

تطبيقك الآن يعمل على **Render**! 🚀

**URL**: `https://restaurant-api.onrender.com`

كل مرة تنشر تحديث إلى GitHub، سيتم نشره تلقائياً.

---

**هل تحتاج مساعدة إضافية؟ اقرأ:**
- [RENDER_SETUP.md](RENDER_SETUP.md) - دليل شامل
- [DEPLOYMENT.md](DEPLOYMENT.md) - نشر عام
- [README.md](README.md) - معلومات المشروع
