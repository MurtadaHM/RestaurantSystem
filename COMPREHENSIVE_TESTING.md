# Comprehensive Testing Guide 🧪

## Pre-Testing Checklist

قبل البدء بالاختبار، تأكد من:
- [ ] Backend قيد التشغيل (`dotnet run`)
- [ ] Database متصل وجاهز
- [ ] Browser console مفتوح (F12)
- [ ] Login credentials متوفر
- [ ] جميع الملفات محفوظة

---

## Test 1: Backend API Readiness

### الخطوات:
```bash
# 1. افتح Terminal
# 2. انتقل إلى المجلد:
cd c:\Users\Murtada\source\repos\RestaurantSystem

# 3. شغّل Backend:
dotnet run --project RestaurantSystem.Api
```

### النتائج المتوقعة:
```
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shutdown.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
info: Microsoft.Hosting.Lifetime[0]
      Content root path: ...\RestaurantSystem.Api
```

### ✅ Success Indicators:
- Backend يعمل بدون أخطاء
- يستمع على port 5183
- Database متصل
- API endpoints accessible

---

## Test 2: Login Flow

### الخطوات:
```
1. افتح Browser
2. اذهب إلى: http://localhost:5183/login.html
3. أدخل:
   - Email: (valid email من Database)
   - Password: (correct password)
4. اضغط "تسجيل الدخول"
```

### ✅ Success Indicators:
- ✅ يتم توجيهك إلى Dashboard.html
- ✅ localStorage['token'] موجود (افتح Console)
  ```javascript
  localStorage.getItem('token')  // يجب أن يرجع JWT token
  ```
- ✅ لا توجد error messages حمراء
- ✅ Header يعرض معلومات المستخدم

### ❌ Failure Cases:
```
❌ "Invalid credentials" - تحقق من email/password في Database
❌ "Network error" - تحقق من Backend
❌ Blank page - تحقق من Console للـ errors
❌ Redirects back to login - Token issue
```

---

## Test 3: Dashboard Loading

### الخطوات:
```
1. بعد Login الناجح
2. تحقق من Dashboard يعرض:
   - الإحصائيات (عدد الطلبات، الإيرادات، إلخ)
   - الرسوم البيانية
   - جدول الطلبات الأخيرة
```

### ✅ Success Indicators:
- ✅ Sidebar عرض على اليسار
- ✅ Stats cards تعرض أرقام
- ✅ Charts تُرسم بدون أخطاء
- ✅ Recent orders table populated

### ❌ Common Issues:
```
❌ Stats showing 0 - API call failed
❌ Charts not rendering - Chart.js issue
❌ Errors in Console - Check network tab
```

### اختبار في Console:
```javascript
// تحقق من جلب البيانات:
fetch('/api/v1/Orders', {
  headers: { 'Authorization': `Bearer ${localStorage.getItem('token')}` }
})
.then(r => r.json())
.then(data => console.log('Orders:', data))
.catch(e => console.error('Error:', e))
```

---

## Test 4: Navigation / Routing System

### الخطوات:
```
1. في Dashboard.html
2. اضغط على كل رابط في الـ sidebar واختبر:
```

#### Test 4.1: Dashboard Link
```
- اضغط: "لوحة التحكم"
- Expected:
  ✅ URL: http://localhost:5183/Dashboard.html#dashboard (أو بدون hash)
  ✅ Header يعرض: "لوحة التحكم الرئيسية"
  ✅ Dashboard view يعرض (not iframe)
  ✅ Charts موجودة
```

#### Test 4.2: Orders Link
```
- اضغط: "الطلبات"
- Expected:
  ✅ URL يتغير: #orders
  ✅ Header يعرض: "الطلبات"
  ✅ iframe يحمّل waiter-dashboard.html
  ✅ جدول الطلبات يظهر
  ✅ في Console لا توجد CORS errors
```

#### Test 4.3: Create Order Link
```
- اضغط: "إنشاء طلب"
- Expected:
  ✅ URL: #waiter-orders
  ✅ Header يعرض: "إنشاء طلب"
  ✅ 3-column layout يظهر (tables, menu, summary)
  ✅ يمكن اختيار tables و menu items
```

#### Test 4.4: Kitchen Link
```
- اضغط: "لوحة المطبخ"
- Expected:
  ✅ URL: #kitchen
  ✅ Order cards تظهر
  ✅ Real-time updates (إذا كان SignalR مفعل)
```

#### Test 4.5: Menu Management
```
- اضغط: "إدارة المنيو"
- Expected:
  ✅ URL: #menu
  ✅ جدول menu items يظهر
  ✅ يمكن إضافة/حذف items (buttons موجودة)
```

#### Test 4.6: Other Links
اختبر بنفس الطريقة:
- Tables (الطاولات)
- Reservations (الحجوزات)
- Categories (الفئات)
- Inventory (المخزون)

### ✅ Success Criteria (for each page):
- [ ] URL hash يتغير
- [ ] Header title يتحدّث
- [ ] Navigation link gets active class
- [ ] Content loads in iframe
- [ ] No CORS errors in Console
- [ ] API data loads (if applicable)

### ❌ Common Failures:
```
❌ Click not working - JavaScript not loaded
❌ Hash doesn't change - navigateTo() not called
❌ iframe blank - Page file not found
❌ CORS error - Backend CORS config needed
❌ 404 errors - Check file paths in pageMap
```

---

## Test 5: Token Accessibility

### في Console (من iframe page):
```javascript
// الاختبار الأول:
console.log('Token:', window.token)
console.log('Shared Token:', window.sharedToken)
console.log('localStorage token:', localStorage.getItem('token'))

// يجب أن يكون واحد منهم موجود
```

### اختبار API Call من iframe:
```javascript
// اختبر من داخل iframe (في صفحة مثل orders)
const token = window.sharedToken || localStorage.getItem('token');
fetch('/api/v1/Orders', {
  headers: { 'Authorization': `Bearer ${token}` }
})
.then(r => r.json())
.then(data => console.log('✅ API Works:', data))
.catch(e => console.error('❌ API Failed:', e))
```

### ✅ Success:
- Token يكون متاح من iframe
- API calls تنجح (status 200)
- Data يُرجع من server

### ❌ Failure:
- Token undefined
- 401 Unauthorized (token invalid)
- 403 Forbidden (permissions issue)

---

## Test 6: Sidebar Duplication

### في Console من iframe page:
```javascript
// كم عدد sidebars؟
const sidebars = document.querySelectorAll('.w-64');
console.log('Sidebars count:', sidebars.length);

// يجب أن يكون 0 (إذا تم الحذف) أو 1 (إذا تم الإبقاء)
// ❌ لا يجب أن يكون 2 أو أكثر
```

### الاختبار البصري:
```
- افتح صفحة (مثل "الطلبات")
- هل ترى sidebar واحد أم اثنين؟
- هل layout مكسور (عناصر فوق بعضها)؟
```

### الحل:
إذا كان هناك 2 sidebars:
- أضف CSS: `.w-64 { display: none; }` في صفحات المحتوى
- أو أزل code الـ sidebar من HTML

---

## Test 7: Form Submission (CRUD Operations)

### Test 7.1: Create Order
```
1. اذهب إلى "إنشاء طلب"
2. اختر table من القائمة
3. اختر menu items
4. اضغط "إضافة طلب"
5. Expected:
   ✅ Toast/alert يظهر "Order created"
   ✅ في Console: POST request successful
   ✅ في Orders page: order جديد يظهر
```

### Test 7.2: Edit Menu Item
```
1. اذهب إلى "إدارة المنيو"
2. اضغط Edit على أي item
3. عدّل البيانات
4. اضغط Save
5. Expected:
   ✅ Modal يُغلق
   ✅ في Console: PUT/PATCH request successful
   ✅ في جدول: data يتحدّث
```

### Test 7.3: Delete Item
```
1. في أي قائمة (menu, tables, etc.)
2. اضغط Delete
3. اضغط Confirm
4. Expected:
   ✅ Item يُزال من جدول
   ✅ في Console: DELETE request successful
   ✅ Success message يظهر
```

### ✅ Success Indicators:
- Network tab يعرض requests صحيح
- Responses هي 200/201/204
- Data في جدول يتحدّث
- No validation errors

### ❌ Failure Cases:
- 400 Bad Request - validation error
- 401 Unauthorized - token issue
- 500 Internal Server Error - backend error
- Network error - connection issue

---

## Test 8: Error Handling

### Test 8.1: Invalid Login
```
1. اذهب إلى Login page
2. أدخل invalid credentials
3. Expected:
   ✅ Error message واضح
   ✅ لا يتم التوجيه إلى Dashboard
```

### Test 8.2: Token Expiry
```
1. Copy token من localStorage
2. عدّل حرف واحد فيه
3. اضغط على أي رابط
4. Expected:
   ✅ API returns 401
   ✅ User redirected to login
```

### Test 8.3: Network Error
```
1. أوقف Backend API
2. حاول تحميل صفحة
3. Expected:
   ✅ Error message يظهر
   ✅ No white/blank screen
   ✅ يمكن محاولة مجددا
```

---

## Test 9: Responsive Design

### على Desktop (1920x1080):
```
✅ Sidebar visible
✅ Content area spacious
✅ Tables readable
✅ Charts rendered well
```

### على Tablet (768px width):
```
✅ Layout still usable
✅ Sidebar width مناسب
✅ Tables scroll horizontally
✅ Fonts readable
```

### على Mobile (375px width):
```
⚠️ Sidebar might hide or stack
⚠️ Content area might be narrow
⚠️ Might need hamburger menu
```

### في Browser DevTools:
```
1. اضغط F12
2. اضغط Ctrl+Shift+M (mobile view)
3. جرّب الأجهزة المختلفة
4. تحقق من responsiveness
```

---

## Test 10: Performance

### قياس Loading Time:

#### في Console:
```javascript
// قبل الضغط على رابط
const start = performance.now();

// ... اضغط على رابط واستنتظر التحميل ...

// بعد التحميل
console.log('Load time:', performance.now() - start, 'ms');

// يجب أن يكون < 2000ms (2 seconds)
```

#### في Browser DevTools:
```
1. اضغط F12 → Network tab
2. اضغط على رابط في Sidebar
3. شاهد الـ waterfall chart
4. Expected:
   ✅ Page loads في < 2 ثانية
   ✅ لا توجد red errors
   ✅ جميع resources تحمّل
```

---

## Test 11: Real-time Updates (SignalR)

### إذا كان SignalR مفعل:

```
1. افتح "لوحة المطبخ" في window
2. افتح صفحة create order في window ثاني
3. اعمل order جديد
4. Expected:
   ✅ في Kitchen page: order جديد يظهر فوراً
   ✅ Real-time بدون refresh
```

### في Console:
```javascript
// تحقق من SignalR connection
// في صفحة Kitchen أو Admin Dashboard
console.log(connection?.state)  // يجب أن يكون "Connected"
```

---

## Test 12: Session Management

### اختبار Logout:
```
1. في Dashboard
2. اضغط على زر Logout (في user profile section)
3. Expected:
   ✅ Redirected إلى login.html
   ✅ localStorage['token'] يُحذف
   ✅ تحقق في Console: localStorage.getItem('token') = null
```

### اختبار Browser Back Button:
```
1. في Dashboard
2. اضغط عدة روابط (orders, menu, etc.)
3. اضغط Browser back button
4. Expected:
   ✅ يرجع للصفحة السابقة
   ✅ URL hash يتغير
   ✅ Content يتحمّل صحيح
```

---

## Debugging Checklist

إذا شيء لم يعمل، افحص:

### 1. Console Errors (F12):
```
- Uncaught ReferenceError
- Uncaught TypeError
- CORS errors
- 404 Not Found
```

### 2. Network Tab (F12):
```
- API requests status (200, 404, 500, etc.)
- Response content
- Headers (Authorization header present?)
- Response time
```

### 3. Local Storage:
```javascript
localStorage.getItem('token')        // Token present?
localStorage.getItem('userData')     // User info present?
sessionStorage.getItem('draftOrder') // Draft data?
```

### 4. Browser Cache:
```
- Hard refresh: Ctrl+Shift+R
- Clear cache: Ctrl+H → Clear browsing data
- Try Incognito mode
```

### 5. Backend Logs:
```bash
# شاهد Terminal حيث يعمل Backend
# يجب أن ترى requests logs
GET /api/v1/Orders - 200
POST /api/v1/Orders - 201
```

---

## Test Results Template

استخدم هذا للتوثيق:

```markdown
# Test Results - Date: [DATE]

## Backend Status
- [ ] API Running
- [ ] Database Connected
- [ ] No Errors

## Frontend Status
- [ ] Login Works
- [ ] Dashboard Loads
- [ ] Navigation Works (8/9 pages tested)
  - [ ] Dashboard
  - [ ] Orders
  - [ ] Create Order
  - [ ] Kitchen
  - [ ] Menu
  - [ ] Tables
  - [ ] Reservations
  - [ ] Categories
  - [ ] Inventory
- [ ] Sidebar Duplication: FIXED / IN PROGRESS
- [ ] Token Access: WORKING / BROKEN
- [ ] API Calls: SUCCESS / FAILURE
- [ ] Forms: WORKING / BROKEN

## Known Issues
- Issue #1: ...
- Issue #2: ...

## Performance
- Average Load Time: ____ ms
- Slowest Page: ____ (___ms)
- Fastest Page: ____ (___ms)

## Ready for Production?
- [ ] All tests passed
- [ ] No critical issues
- [ ] Performance acceptable
- [ ] Mobile tested
```

---

## خلاصة

عند انتهاء الاختبار، يجب أن تكون:

✅ البنية الأساسية تعمل
✅ جميع الروابط تنقل صحيح
✅ بيانات تُحمّل من API
✅ Sidebar مظهره صحيح
✅ لا توجد errors كبيرة

إذا كل هذا ✅، فأنت جاهز للـ:
- إضافة تحسينات إضافية
- تحسين الأداء
- إطلاق الـ production

---

**ملاحظة**: هذا الـ guide يغطي ~80% من الـ use cases. لـ edge cases، قد تحتاج اختبار إضافي.
