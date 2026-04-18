# ⚡ FINAL CHECKLIST - جاهز للإطلاق!

## ✅ قبل الرفع على GitHub

- [x] appsettings.json - قيم صحيحة ✅
- [x] Dockerfile - محسّن ✅
- [x] docker-compose.yaml - جاهز ✅
- [x] render.yaml - موجود ✅
- [x] .env.example - موجود ✅
- [x] .env.prod.example - موجود ✅
- [x] .gitignore - يستثني .env ✅
- [x] GitHub Actions - موجود ✅
- [x] البناء - بدون أخطاء ✅

---

## 🚀 الآن: رفع على GitHub

```bash
cd c:\Users\Murtada\source\repos\RestaurantSystem

git add .
git commit -m "chore: production ready - Docker & Render configured"
git push origin main
```

**⏱️ سيستغرق 30 ثانية**

---

## 🎯 ثم: إعداد Render (من الويب)

### في https://render.com:

1. **New Web Service** (الزر الأزرق)
2. **Select Repository**: MurtadaHM/RestaurantSystem
3. **Settings**:
   - Name: `restaurant-api`
   - Region: `Frankfurt`
   - Plan: `Free`
4. **Environment Variables**: أضف هذه الـ 15 متغير:

```env
# Database (اختر الخيار الأنسب لك)
DB_HOST=your_host
DB_PORT=5432
DB_NAME=restaurant_prod
DB_USER=postgres
DB_PASSWORD=your_password

# JWT
JWT_KEY=your_jwt_key

# Claude
CLAUDE_API_KEY=your_key

# Sendy
SENDY_BASE_URL=https://sendy-backend-production.up.railway.app/
SENDY_API_KEY=your_key
SENDY_WEBHOOK_SECRET=your_secret

# Team6
TEAM6_ENABLED=true
TEAM6_BASE_URL=https://rest-back-qoxj.onrender.com
TEAM6_RESTAURANT_ID=21111111-1111-1111-1111-111111111112
TEAM6_POLLING_INTERVAL=10
TEAM6_FALLBACK_USER_ID=eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee

# App
LOG_LEVEL=Warning
ASPNETCORE_ENVIRONMENT=Production
```

5. **Deploy** (الزر الأزرق)

**⏱️ سيستغرق 5-7 دقائق**

---

## ✅ بعد النشر

```bash
# 1. اختبر الـ API
curl https://restaurant-api.onrender.com/swagger

# 2. من Render Shell، طبّق الـ Migrations:
dotnet ef database update \
  --project RestaurantSystem.Infrastructure \
  --startup-project RestaurantSystem.Api
```

---

## 🎉 تمام!

```
API متاح على:
https://restaurant-api.onrender.com
```

---

## 📞 من الآن فصاعداً

كل مرة تفعل update محلي:

```bash
git add .
git commit -m "feat: your feature"
git push origin main
# سيفعّل النشر التلقائي على Render! 🚀
```

---

**تم! 🎊 نظامك الآن في الإنتاج!**
