# 🍽️ Restaurant System - Frontend Integration Guide

## 📌 نظرة عامة

هذا الدليل موجه لـ **فريق الفرونت إند** للتكامل مع **Restaurant API**.

---

## 🌐 API Base URLs

| البيئة | الـ URL | الحالة |
|--------|---------|--------|
| **Local Development** | `http://localhost:5183` | Development |
| **Production (Render)** | `https://restaurantsystem-oe83.onrender.com` | Live 🚀 |

---

## 🔐 Authentication

### 1️⃣ Login Endpoint

**Request:**
```bash
POST /api/v1/Auth/login
Content-Type: application/json

{
  "email": "admin@restaurant.com",
  "password": "your_password"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Login successful",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "user": {
      "id": "user-uuid",
      "email": "admin@restaurant.com",
      "fullName": "Admin User",
      "role": "Admin"
    }
  }
}
```

### 2️⃣ استخدام الـ Token

كل طلب يحتاج **Authorization Header**:

```javascript
const token = response.data.token;

// في axios
axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;

// أو في كل طلب
fetch('/api/v1/Auth/profile', {
  headers: {
    'Authorization': `Bearer ${token}`
  }
})
```

---

## 📊 الـ Endpoints الأساسية

### **1. Auth (المصادقة)**

| Method | Endpoint | الوصف |
|--------|----------|--------|
| `POST` | `/api/v1/Auth/login` | تسجيل دخول |
| `POST` | `/api/v1/Auth/register` | إنشاء حساب |
| `GET` | `/api/v1/Auth/profile` | بيانات المستخدم الحالي |
| `POST` | `/api/v1/Auth/logout` | تسجيل خروج |

### **2. Tables (الطاولات)**

| Method | Endpoint | الوصف |
|--------|----------|--------|
| `GET` | `/api/v1/Tables` | كل الطاولات |
| `GET` | `/api/v1/Tables/{id}` | طاولة واحدة |
| `POST` | `/api/v1/Tables` | إضافة طاولة |
| `PUT` | `/api/v1/Tables/{id}` | تعديل طاولة |
| `DELETE` | `/api/v1/Tables/{id}` | حذف طاولة |

### **3. Menu (القائمة)**

| Method | Endpoint | الوصف |
|--------|----------|--------|
| `GET` | `/api/v1/Menu` | كل المنتجات |
| `GET` | `/api/v1/Menu/{id}` | منتج واحد |
| `POST` | `/api/v1/Menu` | إضافة منتج |
| `PUT` | `/api/v1/Menu/{id}` | تعديل منتج |
| `DELETE` | `/api/v1/Menu/{id}` | حذف منتج |

### **4. Orders (الطلبات)**

| Method | Endpoint | الوصف |
|--------|----------|--------|
| `GET` | `/api/v1/Orders` | كل الطلبات |
| `GET` | `/api/v1/Orders/{id}` | طلب واحد |
| `POST` | `/api/v1/Orders` | إنشاء طلب |
| `PUT` | `/api/v1/Orders/{id}` | تعديل طلب |
| `DELETE` | `/api/v1/Orders/{id}` | حذف طلب |
| `PUT` | `/api/v1/Orders/{id}/status` | تحديث حالة الطلب |

### **5. Payments (الدفع)**

| Method | Endpoint | الوصف |
|--------|----------|--------|
| `GET` | `/api/v1/Payments` | كل المدفوعات |
| `POST` | `/api/v1/Payments` | إضافة دفع |
| `GET` | `/api/v1/Payments/{id}` | دفع واحد |

### **6. Reservations (الحجوزات)**

| Method | Endpoint | الوصف |
|--------|----------|--------|
| `GET` | `/api/v1/Reservations` | كل الحجوزات |
| `POST` | `/api/v1/Reservations` | حجز طاولة |
| `PUT` | `/api/v1/Reservations/{id}` | تعديل حجز |
| `DELETE` | `/api/v1/Reservations/{id}` | إلغاء حجز |

---

## 💻 أمثلة على الاستخدام

### **JavaScript/TypeScript (Axios)**

```typescript
import axios from 'axios';

const API_URL = 'https://restaurantsystem-oe83.onrender.com';

const api = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json'
  }
});

// Login
async function login(email: string, password: string) {
  try {
    const response = await api.post('/api/v1/Auth/login', {
      email,
      password
    });
    
    const token = response.data.data.token;
    localStorage.setItem('token', token);
    api.defaults.headers.common['Authorization'] = `Bearer ${token}`;
    
    return response.data;
  } catch (error) {
    console.error('Login failed:', error);
  }
}

// Get All Tables
async function getTables() {
  try {
    const response = await api.get('/api/v1/Tables');
    return response.data.data;
  } catch (error) {
    console.error('Failed to fetch tables:', error);
  }
}

// Create Order
async function createOrder(tableId: string, items: any[]) {
  try {
    const response = await api.post('/api/v1/Orders', {
      tableId,
      items
    });
    return response.data.data;
  } catch (error) {
    console.error('Failed to create order:', error);
  }
}
```

### **React Hook Example**

```typescript
import { useEffect, useState } from 'react';
import axios from 'axios';

const API_URL = 'https://restaurantsystem-oe83.onrender.com';

export function useAuth() {
  const [user, setUser] = useState(null);
  const [token, setToken] = useState(localStorage.getItem('token'));
  
  const login = async (email: string, password: string) => {
    try {
      const response = await axios.post(`${API_URL}/api/v1/Auth/login`, {
        email,
        password
      });
      
      const newToken = response.data.data.token;
      setToken(newToken);
      setUser(response.data.data.user);
      localStorage.setItem('token', newToken);
      
      return response.data;
    } catch (error) {
      console.error('Login failed:', error);
      throw error;
    }
  };
  
  const logout = () => {
    setToken(null);
    setUser(null);
    localStorage.removeItem('token');
  };
  
  return { user, token, login, logout };
}

export function useTables() {
  const [tables, setTables] = useState([]);
  const token = localStorage.getItem('token');
  
  useEffect(() => {
    const fetchTables = async () => {
      try {
        const response = await axios.get(`${API_URL}/api/v1/Tables`, {
          headers: {
            'Authorization': `Bearer ${token}`
          }
        });
        setTables(response.data.data);
      } catch (error) {
        console.error('Failed to fetch tables:', error);
      }
    };
    
    if (token) {
      fetchTables();
    }
  }, [token]);
  
  return tables;
}
```

---

## 🔄 Real-time Updates (SignalR)

### **Connection Setup**

```typescript
import * as signalR from '@microsoft/signalr';

const connection = new signalR.HubConnectionBuilder()
  .withUrl('https://restaurantsystem-oe83.onrender.com/orderHub', {
    accessTokenFactory: () => localStorage.getItem('token') || ''
  })
  .withAutomaticReconnect()
  .build();

connection.start().catch(err => console.error('Connection failed:', err));

// Listen for order updates
connection.on('OrderStatusUpdated', (orderId, status) => {
  console.log(`Order ${orderId} status: ${status}`);
  // Update UI here
});

// Listen for new orders
connection.on('NewOrderCreated', (order) => {
  console.log('New order:', order);
  // Update UI here
});
```

---

## 🧪 اختبار الـ API

### **استخدام Swagger (Interactive)**

```
https://restaurantsystem-oe83.onrender.com/swagger/index.html
```

### **استخدام cURL**

```bash
# Login
curl -X POST https://restaurantsystem-oe83.onrender.com/api/v1/Auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@restaurant.com",
    "password": "password"
  }'

# Get Tables (مع Token)
curl -X GET https://restaurantsystem-oe83.onrender.com/api/v1/Tables \
  -H "Authorization: Bearer YOUR_TOKEN"
```

---

## 🛠️ Setup للفرونت إند

### **1. تثبيت Dependencies**

```bash
npm install axios @microsoft/signalr
```

### **2. Configuration**

```typescript
// src/config/api.ts
export const API_CONFIG = {
  baseURL: process.env.REACT_APP_API_URL || 'https://restaurantsystem-oe83.onrender.com',
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json'
  }
};
```

### **3. Environment Variables (.env)**

```
REACT_APP_API_URL=https://restaurantsystem-oe83.onrender.com
REACT_APP_SOCKET_URL=https://restaurantsystem-oe83.onrender.com/orderHub
```

---

## 📋 Error Handling

```typescript
interface ApiError {
  success: false;
  message: string;
  errors?: Record<string, string[]>;
}

// استخدام
try {
  const response = await api.post('/api/v1/Orders', data);
} catch (error: any) {
  if (error.response?.status === 401) {
    // Unauthorized - إعادة توجيه للـ login
    window.location.href = '/login';
  } else if (error.response?.status === 400) {
    // Validation errors
    const errors = error.response.data.errors;
    console.error('Validation errors:', errors);
  } else {
    // General error
    console.error('Error:', error.response?.data?.message);
  }
}
```

---

## 📞 الدعم والتواصل

- **API Documentation:** `/swagger/index.html`
- **Base URL:** `https://restaurantsystem-oe83.onrender.com`
- **Status:** 🟢 Live and Running

---

**آخر تحديث:** April 18, 2026
