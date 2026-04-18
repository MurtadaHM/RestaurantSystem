# 📚 Restaurant System - API Data Models & Schemas

## 👤 User Model

```typescript
interface User {
  id: string;           // UUID
  email: string;        // البريد الإلكتروني (فريد)
  fullName: string;     // الاسم الكامل
  phoneNumber: string;  // رقم الهاتف
  role: string;         // Admin, Manager, Waiter, Chef
  department?: string;  // القسم (المطبخ، الخدمة، إلخ)
  isActive: boolean;    // هل الحساب نشط
  createdAt: string;    // تاريخ الإنشاء (ISO 8601)
  updatedAt: string;    // تاريخ التحديث
}
```

---

## 🍽️ Table Model

```typescript
interface Table {
  id: string;              // UUID
  name: string;            // اسم الطاولة (مثل: A1, B2)
  capacity: number;        // السعة (عدد الأشخاص)
  location: string;        // الموقع (بالقرب من النافذة، إلخ)
  isAvailable: boolean;    // هل الطاولة متاحة
  currentOrderId?: string; // الطلب الحالي
  createdAt: string;
  updatedAt: string;
}
```

**مثال:**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "name": "A1",
  "capacity": 4,
  "location": "بالقرب من النافذة",
  "isAvailable": true,
  "createdAt": "2026-04-18T10:30:00Z"
}
```

---

## 📋 Menu Item Model

```typescript
interface MenuItem {
  id: string;                // UUID
  name: string;              // اسم المنتج
  description: string;       // الوصف
  price: number;             // السعر
  category: string;          // الفئة (مشروبات، حلويات، إلخ)
  image?: string;            // صورة المنتج (URL)
  isAvailable: boolean;      // هل المنتج متاح
  preparationTime: number;   // وقت التحضير (دقائق)
  calories?: number;         // السعرات الحرارية
  ingredients: string[];     // المكونات
  allergens?: string[];      // المواد المسببة للحساسية
  createdAt: string;
  updatedAt: string;
}
```

**مثال:**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440001",
  "name": "برجر لحم",
  "description": "برجر لذيذ مع جبنة وطماطم",
  "price": 45.00,
  "category": "الوجبات الرئيسية",
  "image": "https://example.com/burger.jpg",
  "isAvailable": true,
  "preparationTime": 15,
  "calories": 650,
  "ingredients": ["لحم", "خبز", "جبنة", "طماطم", "خس"],
  "allergens": ["غلوتين", "لبن"]
}
```

---

## 🛒 Order Model

```typescript
interface OrderItem {
  id: string;           // UUID
  menuItemId: string;   // معرف المنتج
  quantity: number;     // الكمية
  unitPrice: number;    // سعر الوحدة
  totalPrice: number;   // السعر الإجمالي
  notes?: string;       // ملاحظات (بدون بصل، إلخ)
}

interface Order {
  id: string;              // UUID
  tableId: string;         // معرف الطاولة
  items: OrderItem[];      // قائمة العناصر
  status: string;          // Pending, Preparing, Ready, Served, Completed, Cancelled
  subtotal: number;        // المجموع الجزئي
  tax: number;             // الضريبة
  total: number;           // المجموع النهائي
  notes?: string;          // ملاحظات الطلب
  createdAt: string;
  updatedAt: string;
  servedAt?: string;       // وقت التقديم
  completedAt?: string;    // وقت الإكمال
}
```

**مثال:**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440002",
  "tableId": "550e8400-e29b-41d4-a716-446655440000",
  "items": [
    {
      "id": "item-1",
      "menuItemId": "550e8400-e29b-41d4-a716-446655440001",
      "quantity": 2,
      "unitPrice": 45.00,
      "totalPrice": 90.00,
      "notes": "بدون بصل"
    }
  ],
  "status": "Preparing",
  "subtotal": 90.00,
  "tax": 13.50,
  "total": 103.50,
  "createdAt": "2026-04-18T12:00:00Z",
  "updatedAt": "2026-04-18T12:05:00Z"
}
```

---

## 💳 Payment Model

```typescript
interface Payment {
  id: string;           // UUID
  orderId: string;      // معرف الطلب
  amount: number;       // المبلغ
  method: string;       // Cash, Card, Wallet
  status: string;       // Pending, Completed, Failed, Cancelled
  reference?: string;   // مرجع الدفع (رقم التحويل، إلخ)
  notes?: string;       // ملاحظات
  createdAt: string;
  processedAt?: string; // وقت معالجة الدفع
}
```

**مثال:**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440003",
  "orderId": "550e8400-e29b-41d4-a716-446655440002",
  "amount": 103.50,
  "method": "Card",
  "status": "Completed",
  "reference": "TXN-12345-67890",
  "createdAt": "2026-04-18T12:30:00Z",
  "processedAt": "2026-04-18T12:30:15Z"
}
```

---

## 📅 Reservation Model

```typescript
interface Reservation {
  id: string;              // UUID
  guestName: string;       // اسم الضيف
  guestPhone: string;      // رقم الهاتف
  guestEmail: string;      // البريد الإلكتروني
  tableId: string;         // معرف الطاولة
  numberOfGuests: number;  // عدد الأشخاص
  reservationTime: string; // وقت الحجز (ISO 8601)
  status: string;          // Pending, Confirmed, Cancelled, Completed
  notes?: string;          // ملاحظات خاصة
  createdAt: string;
  updatedAt: string;
}
```

**مثال:**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440004",
  "guestName": "أحمد محمد",
  "guestPhone": "+966501234567",
  "guestEmail": "ahmed@example.com",
  "tableId": "550e8400-e29b-41d4-a716-446655440000",
  "numberOfGuests": 4,
  "reservationTime": "2026-04-18T19:00:00Z",
  "status": "Confirmed",
  "notes": "زفاف خاص - يفضل الطاولة القريبة من المسرح",
  "createdAt": "2026-04-17T14:30:00Z"
}
```

---

## 🔍 Category Model

```typescript
interface Category {
  id: string;        // UUID
  name: string;      // اسم الفئة
  icon?: string;     // أيقونة (emoji أو URL)
  description?: string;
  displayOrder: number; // ترتيب العرض
  createdAt: string;
}
```

---

## 📊 Department Model

```typescript
interface Department {
  id: string;        // UUID
  name: string;      // اسم القسم (مطبخ، خدمة، إلخ)
  description?: string;
  manager?: string;  // معرف مدير القسم
  createdAt: string;
}
```

---

## 📡 API Response Format

### ✅ Success Response

```typescript
interface ApiSuccessResponse<T> {
  success: true;
  message: string;
  data: T;
  timestamp: string; // ISO 8601
}
```

**مثال:**
```json
{
  "success": true,
  "message": "تم جلب الطاولات بنجاح",
  "data": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "name": "A1",
      "capacity": 4,
      "isAvailable": true
    }
  ],
  "timestamp": "2026-04-18T12:00:00Z"
}
```

### ❌ Error Response

```typescript
interface ApiErrorResponse {
  success: false;
  message: string;
  errors?: Record<string, string[]>;
  timestamp: string;
}
```

**مثال (Validation Error):**
```json
{
  "success": false,
  "message": "فشل التحقق من صحة البيانات",
  "errors": {
    "email": ["البريد الإلكتروني مطلوب", "يجب أن يكون بريداً إلكترونياً صحيحاً"],
    "password": ["كلمة المرور يجب أن تكون 8 أحرف على الأقل"]
  },
  "timestamp": "2026-04-18T12:00:00Z"
}
```

---

## 🔐 Authentication Response

```typescript
interface LoginResponse {
  success: true;
  message: string;
  data: {
    token: string;           // JWT Token
    user: {
      id: string;
      email: string;
      fullName: string;
      role: string;
    }
  }
}
```

---

## 📝 Order Status Flow

```
Pending (منتظر)
    ↓
Preparing (قيد التحضير)
    ↓
Ready (جاهز)
    ↓
Served (تم التقديم)
    ↓
Completed (مكتمل) ✓

أو في أي وقت:
    ↓
Cancelled (ملغى) ✗
```

---

## 🔄 Real-time Events (SignalR)

### **Order Events**

```typescript
// عند إنشاء طلب جديد
event: "NewOrderCreated"
data: Order

// عند تحديث حالة الطلب
event: "OrderStatusUpdated"
data: { orderId: string, status: string }

// عند حذف طلب
event: "OrderDeleted"
data: { orderId: string }

// عند إضافة عنصر للطلب
event: "OrderItemAdded"
data: { orderId: string, item: OrderItem }

// عند إزالة عنصر من الطلب
event: "OrderItemRemoved"
data: { orderId: string, itemId: string }
```

---

## 🎯 Status Codes

```
200 OK              - الطلب نجح
201 Created         - تم الإنشاء
204 No Content      - نجح بدون محتوى
400 Bad Request     - طلب خاطئ
401 Unauthorized    - غير مصرح (يحتاج تسجيل دخول)
403 Forbidden       - محظور (صلاحيات غير كافية)
404 Not Found       - غير موجود
409 Conflict        - تضارب (مثل تكرار البريد)
422 Unprocessable   - فشل التحقق
429 Too Many        - طلبات كثيرة جداً
500 Server Error    - خطأ في الخادم
```

---

**آخر تحديث:** April 18, 2026
