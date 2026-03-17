# 🍽️ Restaurant Management System (Clean Architecture)

نظام متكامل لإدارة المطاعم مبني باستخدام تقنيات .NET الحديثة، يتبع نمط العمارة النظيفة لضمان القابلية للتوسع والصيانة.

## 🛠️ التقنيات المستخدمة (Tech Stack)
* **Backend:** .NET 8 Web API
* **Architecture:** Clean Architecture (Domain, Application, Infrastructure, API)
* **Database:** PostgreSQL with Entity Framework Core
* **Security:** JWT Authentication & Role-based Authorization
* **Tools:** AutoMapper, FluentValidation, Swagger (OpenAPI)

## ✨ المميزات الرئيسية (Features)
* **User Management:** نظام تسجيل دخول وحماية باستخدام JWT.
* **Order System:** إدارة الطلبات (داخلي، سفري، توصيل) مع حساب تلقائي للمبالغ.
* **Menu Management:** التحكم الكامل بالأصناف والأسعار.
* **Table Tracking:** مراقبة حالة الطاولات (متاحة/محجوزة).
* **AI Diagnostics:** دمج أولي لخدمات الذكاء الاصطناعي لتشخيص حالة النظام.

## 🚀 كيفية التشغيل (Setup)
1. قم بعمل Clone للمستودع.
2. حدث بيانات قاعدة البيانات في ملف `appsettings.json`.
3. نفذ الأمر `Update-Database` لتوليد الجداول.
4. اضغط Run واستمتع باستكشاف الـ API عبر Swagger.
