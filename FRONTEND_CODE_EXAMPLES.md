# 🚀 Restaurant API - Quick Start Examples

## 📦 Installation

```bash
npm install axios @microsoft/signalr
```

---

## 🔧 API Service Configuration

### **api.service.ts**

```typescript
import axios, { AxiosInstance } from 'axios';

class ApiService {
  private api: AxiosInstance;
  
  constructor() {
    this.api = axios.create({
      baseURL: process.env.REACT_APP_API_URL || 'https://restaurantsystem-oe83.onrender.com',
      headers: {
        'Content-Type': 'application/json',
      },
    });
    
    // Add token to every request
    this.api.interceptors.request.use((config) => {
      const token = localStorage.getItem('token');
      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
      }
      return config;
    });
    
    // Handle errors
    this.api.interceptors.response.use(
      (response) => response,
      (error) => {
        if (error.response?.status === 401) {
          localStorage.removeItem('token');
          window.location.href = '/login';
        }
        return Promise.reject(error);
      }
    );
  }
  
  // Auth
  login(email: string, password: string) {
    return this.api.post('/api/v1/Auth/login', { email, password });
  }
  
  register(data: any) {
    return this.api.post('/api/v1/Auth/register', data);
  }
  
  getProfile() {
    return this.api.get('/api/v1/Auth/profile');
  }
  
  // Tables
  getTables() {
    return this.api.get('/api/v1/Tables');
  }
  
  getTable(id: string) {
    return this.api.get(`/api/v1/Tables/${id}`);
  }
  
  createTable(data: any) {
    return this.api.post('/api/v1/Tables', data);
  }
  
  updateTable(id: string, data: any) {
    return this.api.put(`/api/v1/Tables/${id}`, data);
  }
  
  deleteTable(id: string) {
    return this.api.delete(`/api/v1/Tables/${id}`);
  }
  
  // Menu
  getMenuItems() {
    return this.api.get('/api/v1/Menu');
  }
  
  getMenuItem(id: string) {
    return this.api.get(`/api/v1/Menu/${id}`);
  }
  
  createMenuItem(data: any) {
    return this.api.post('/api/v1/Menu', data);
  }
  
  updateMenuItem(id: string, data: any) {
    return this.api.put(`/api/v1/Menu/${id}`, data);
  }
  
  deleteMenuItem(id: string) {
    return this.api.delete(`/api/v1/Menu/${id}`);
  }
  
  // Orders
  getOrders() {
    return this.api.get('/api/v1/Orders');
  }
  
  getOrder(id: string) {
    return this.api.get(`/api/v1/Orders/${id}`);
  }
  
  createOrder(data: any) {
    return this.api.post('/api/v1/Orders', data);
  }
  
  updateOrder(id: string, data: any) {
    return this.api.put(`/api/v1/Orders/${id}`, data);
  }
  
  updateOrderStatus(id: string, status: string) {
    return this.api.put(`/api/v1/Orders/${id}/status`, { status });
  }
  
  deleteOrder(id: string) {
    return this.api.delete(`/api/v1/Orders/${id}`);
  }
  
  // Payments
  createPayment(data: any) {
    return this.api.post('/api/v1/Payments', data);
  }
  
  getPayments() {
    return this.api.get('/api/v1/Payments');
  }
  
  // Reservations
  getReservations() {
    return this.api.get('/api/v1/Reservations');
  }
  
  createReservation(data: any) {
    return this.api.post('/api/v1/Reservations', data);
  }
  
  updateReservation(id: string, data: any) {
    return this.api.put(`/api/v1/Reservations/${id}`, data);
  }
  
  deleteReservation(id: string) {
    return this.api.delete(`/api/v1/Reservations/${id}`);
  }
}

export default new ApiService();
```

---

## ⚛️ React Hooks Examples

### **useAuth.ts**

```typescript
import { useState, useCallback } from 'react';
import apiService from './api.service';

export function useAuth() {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  
  const login = useCallback(async (email: string, password: string) => {
    setLoading(true);
    setError(null);
    
    try {
      const response = await apiService.login(email, password);
      const token = response.data.data.token;
      const user = response.data.data.user;
      
      localStorage.setItem('token', token);
      setUser(user);
      
      return { success: true, user };
    } catch (err: any) {
      const errorMessage = err.response?.data?.message || 'Login failed';
      setError(errorMessage);
      return { success: false, error: errorMessage };
    } finally {
      setLoading(false);
    }
  }, []);
  
  const logout = useCallback(() => {
    localStorage.removeItem('token');
    setUser(null);
  }, []);
  
  const register = useCallback(async (userData: any) => {
    setLoading(true);
    setError(null);
    
    try {
      const response = await apiService.register(userData);
      return { success: true, data: response.data.data };
    } catch (err: any) {
      const errorMessage = err.response?.data?.message || 'Registration failed';
      setError(errorMessage);
      return { success: false, error: errorMessage };
    } finally {
      setLoading(false);
    }
  }, []);
  
  return { user, loading, error, login, logout, register };
}
```

### **useTables.ts**

```typescript
import { useState, useEffect } from 'react';
import apiService from './api.service';

export function useTables() {
  const [tables, setTables] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  
  useEffect(() => {
    const fetchTables = async () => {
      setLoading(true);
      try {
        const response = await apiService.getTables();
        setTables(response.data.data);
      } catch (err: any) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };
    
    fetchTables();
  }, []);
  
  const createTable = async (data: any) => {
    try {
      const response = await apiService.createTable(data);
      setTables([...tables, response.data.data]);
      return { success: true, data: response.data.data };
    } catch (err: any) {
      return { success: false, error: err.message };
    }
  };
  
  const updateTable = async (id: string, data: any) => {
    try {
      const response = await apiService.updateTable(id, data);
      setTables(tables.map(t => t.id === id ? response.data.data : t));
      return { success: true, data: response.data.data };
    } catch (err: any) {
      return { success: false, error: err.message };
    }
  };
  
  const deleteTable = async (id: string) => {
    try {
      await apiService.deleteTable(id);
      setTables(tables.filter(t => t.id !== id));
      return { success: true };
    } catch (err: any) {
      return { success: false, error: err.message };
    }
  };
  
  return { tables, loading, error, createTable, updateTable, deleteTable };
}
```

### **useOrders.ts**

```typescript
import { useState, useEffect } from 'react';
import apiService from './api.service';

export function useOrders() {
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  
  useEffect(() => {
    const fetchOrders = async () => {
      setLoading(true);
      try {
        const response = await apiService.getOrders();
        setOrders(response.data.data);
      } catch (err: any) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };
    
    fetchOrders();
  }, []);
  
  const createOrder = async (orderData: any) => {
    try {
      const response = await apiService.createOrder(orderData);
      setOrders([...orders, response.data.data]);
      return { success: true, data: response.data.data };
    } catch (err: any) {
      return { success: false, error: err.response?.data?.message || err.message };
    }
  };
  
  const updateOrderStatus = async (id: string, status: string) => {
    try {
      const response = await apiService.updateOrderStatus(id, status);
      setOrders(orders.map(o => o.id === id ? { ...o, status } : o));
      return { success: true, data: response.data.data };
    } catch (err: any) {
      return { success: false, error: err.message };
    }
  };
  
  return { orders, loading, error, createOrder, updateOrderStatus };
}
```

---

## 🔌 SignalR Real-time Connection

### **orderHub.service.ts**

```typescript
import * as signalR from '@microsoft/signalr';

class OrderHubService {
  private connection: signalR.HubConnection | null = null;
  
  async connect(): Promise<void> {
    const token = localStorage.getItem('token');
    
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl('https://restaurantsystem-oe83.onrender.com/orderHub', {
        accessTokenFactory: () => token || '',
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
      })
      .withAutomaticReconnect()
      .build();
    
    await this.connection.start();
    console.log('Connected to OrderHub');
  }
  
  disconnect(): void {
    if (this.connection) {
      this.connection.stop();
    }
  }
  
  onOrderStatusUpdated(callback: (orderId: string, status: string) => void): void {
    if (this.connection) {
      this.connection.on('OrderStatusUpdated', callback);
    }
  }
  
  onNewOrderCreated(callback: (order: any) => void): void {
    if (this.connection) {
      this.connection.on('NewOrderCreated', callback);
    }
  }
  
  onOrderDeleted(callback: (orderId: string) => void): void {
    if (this.connection) {
      this.connection.on('OrderDeleted', callback);
    }
  }
}

export default new OrderHubService();
```

---

## 📱 Complete React Component Example

### **LoginPage.tsx**

```typescript
import React, { useState } from 'react';
import { useAuth } from './hooks/useAuth';

export function LoginPage() {
  const { login, loading, error } = useAuth();
  const [email, setEmail] = useState('admin@restaurant.com');
  const [password, setPassword] = useState('');
  
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const result = await login(email, password);
    
    if (result.success) {
      window.location.href = '/dashboard';
    }
  };
  
  return (
    <div className="login-container">
      <h2>تسجيل الدخول</h2>
      
      {error && <div className="error">{error}</div>}
      
      <form onSubmit={handleSubmit}>
        <input
          type="email"
          placeholder="البريد الإلكتروني"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
        />
        
        <input
          type="password"
          placeholder="كلمة المرور"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
        />
        
        <button type="submit" disabled={loading}>
          {loading ? 'جاري...' : 'دخول'}
        </button>
      </form>
    </div>
  );
}
```

### **TablesPage.tsx**

```typescript
import React, { useState } from 'react';
import { useTables } from './hooks/useTables';

export function TablesPage() {
  const { tables, loading, error, createTable, deleteTable } = useTables();
  const [newTableName, setNewTableName] = useState('');
  
  const handleAddTable = async () => {
    if (newTableName.trim()) {
      const result = await createTable({ name: newTableName, capacity: 4 });
      if (result.success) {
        setNewTableName('');
      }
    }
  };
  
  if (loading) return <div>جاري التحميل...</div>;
  if (error) return <div>خطأ: {error}</div>;
  
  return (
    <div className="tables-page">
      <h2>الطاولات</h2>
      
      <div className="add-table">
        <input
          type="text"
          placeholder="اسم الطاولة"
          value={newTableName}
          onChange={(e) => setNewTableName(e.target.value)}
        />
        <button onClick={handleAddTable}>إضافة طاولة</button>
      </div>
      
      <div className="tables-list">
        {tables.map(table => (
          <div key={table.id} className="table-card">
            <h3>{table.name}</h3>
            <p>السعة: {table.capacity}</p>
            <p>الحالة: {table.isAvailable ? 'متاحة' : 'مشغولة'}</p>
            <button onClick={() => deleteTable(table.id)}>حذف</button>
          </div>
        ))}
      </div>
    </div>
  );
}
```

---

## 🧪 Testing with cURL

```bash
# Login
curl -X POST https://restaurantsystem-oe83.onrender.com/api/v1/Auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@restaurant.com",
    "password": "password123"
  }'

# Get Tables
TOKEN="your_token_here"
curl -X GET https://restaurantsystem-oe83.onrender.com/api/v1/Tables \
  -H "Authorization: Bearer $TOKEN"

# Create Order
curl -X POST https://restaurantsystem-oe83.onrender.com/api/v1/Orders \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "tableId": "table-uuid",
    "items": [
      {
        "menuItemId": "item-uuid",
        "quantity": 2
      }
    ]
  }'
```

---

**تم التحديث:** April 18, 2026
