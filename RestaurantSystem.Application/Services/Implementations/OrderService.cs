using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using RestaurantSystem.Application.Contracts.Repositories;
using RestaurantSystem.Application.DTOs.Orders;
using RestaurantSystem.Application.Services.Interfaces;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMenuRepository _menuRepository;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, IMenuRepository menuRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _menuRepository = menuRepository;
            _mapper = mapper;
        }

        public async Task<OrderResponseDto> CreateOrderAsync(CreateOrderRequestDto request)
        {
            var order = new Order
            {
                UserId = request.UserId,
                TableId = request.TableId,
                OrderType = request.OrderType,
                SpecialNotes = request.SpecialNotes ?? string.Empty,
                DeliveryFee = request.DeliveryFee,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                OrderItems = new List<OrderItem>()
            };

            foreach (var item in request.Items)
            {
                var menuItem = await _menuRepository.GetByIdAsync(item.MenuItemId);
                if (menuItem == null)
                    throw new Exception($"المنتج {item.MenuItemId} غير موجود");

                // ✅ التعديل هنا: بما أن Price هو decimal (ليس نل)، نأخذه مباشرة بدون دوال إضافية
                decimal priceValue = menuItem.Price;

                order.OrderItems.Add(new OrderItem
                {
                    MenuItemId = item.MenuItemId,
                    Quantity = item.Quantity,
                    Price = priceValue,
                    SpecialInstructions = item.SpecialInstructions ?? string.Empty,
                    CreatedAt = DateTime.UtcNow
                });
            }

            // ✅ حساب المجموع: الـ Sum سيرجع decimal لأن الـ Price والـ Quantity غير نل
            order.TotalAmount = order.OrderItems.Sum(oi => oi.Price * oi.Quantity);

            // ✅ إضافة رسوم التوصيل بحذر لأنها decimal?
            if (order.OrderType == OrderType.Delivery && order.DeliveryFee.HasValue)
            {
                // نستخدم .Value للتحويل الصريح من decimal? إلى decimal
                order.TotalAmount += order.DeliveryFee.Value;
            }

            await _orderRepository.AddAsync(order);

            return _mapper.Map<OrderResponseDto>(order);
        }

        public async Task<OrderResponseDto> GetOrderByIdAsync(Guid id)
        {
            var order = await _orderRepository.GetOrderWithDetailsAsync(id);
            if (order == null) throw new Exception("عذراً، الطلب غير موجود");

            return _mapper.Map<OrderResponseDto>(order);
        }

        public async Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllOrdersWithDetailsAsync();
            return _mapper.Map<IEnumerable<OrderResponseDto>>(orders);
        }

        public async Task<IEnumerable<OrderResponseDto>> GetOrdersByUserIdAsync(string userId)
        {
            if (!Guid.TryParse(userId, out Guid guidUserId))
                throw new Exception("معرف المستخدم غير صحيح");

            var orders = await _orderRepository.GetOrdersByUserIdAsync(guidUserId);
            return _mapper.Map<IEnumerable<OrderResponseDto>>(orders);
        }

        public async Task<IEnumerable<OrderResponseDto>> GetOrdersByTableIdAsync(Guid tableId)
        {
            var orders = await _orderRepository.GetOrdersByTableIdAsync(tableId);
            return _mapper.Map<IEnumerable<OrderResponseDto>>(orders);
        }

        public async Task<OrderResponseDto> UpdateOrderStatusAsync(Guid id, UpdateOrderStatusRequestDto request)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) throw new Exception("الطلب غير موجود");

            order.Status = request.NewStatus;
            order.UpdatedAt = DateTime.UtcNow;

            if (request.NewStatus == OrderStatus.Completed)
                order.CompletedAt = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order);
            return _mapper.Map<OrderResponseDto>(order);
        }

        public async Task<OrderResponseDto> UpdateOrderAsync(Guid id, CreateOrderRequestDto request)
        {
            var order = await _orderRepository.GetOrderWithDetailsAsync(id);
            if (order == null) throw new Exception("الطلب غير موجود");

            order.SpecialNotes = request.SpecialNotes;
            order.OrderType = request.OrderType;
            order.UpdatedAt = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order);
            return _mapper.Map<OrderResponseDto>(order);
        }

        public async Task<bool> DeleteOrderAsync(Guid id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return false;

            await _orderRepository.DeleteAsync(id);
            return true;
        }

        public async Task<decimal> CalculateOrderTotalAsync(Guid orderId)
        {
            var order = await _orderRepository.GetOrderWithDetailsAsync(orderId);
            if (order == null) throw new Exception("الطلب غير موجود");

            return order.OrderItems.Sum(oi => oi.Price * oi.Quantity);
        }

        public async Task<bool> CancelOrderAsync(Guid id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return false;

            if (order.Status != OrderStatus.Pending)
                throw new Exception("لا يمكن إلغاء الطلب في هذه المرحلة");

            order.Status = OrderStatus.Cancelled;
            order.UpdatedAt = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order);
            return true;
        }

        public async Task<IEnumerable<OrderResponseDto>> GetPendingOrdersAsync()
        {
            var orders = await _orderRepository.GetPendingOrdersAsync();
            return _mapper.Map<IEnumerable<OrderResponseDto>>(orders);
        }
    }
}