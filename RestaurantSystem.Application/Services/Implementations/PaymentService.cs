using AutoMapper;
using RestaurantSystem.Application.Contracts.Repositories;
using RestaurantSystem.Application.DTOs.Payments;
using RestaurantSystem.Application.Services.Interfaces;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;
using RestaurantSystem.Domain.Exceptions;

namespace RestaurantSystem.Application.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ITableRepository _tableRepository; // ✅ أضفنا هذا السطر
        private readonly IMapper _mapper;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IOrderRepository orderRepository,
            ITableRepository tableRepository, // ✅ حقن الـ Repository هنا
            IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _orderRepository = orderRepository;
            _tableRepository = tableRepository; // ✅ تعيين القيمة
            _mapper = mapper;
        }

        public async Task<PaymentResponseDto> CreatePaymentAsync(CreatePaymentRequestDto request)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId);
            if (order == null)
                throw new NotFoundException("Order", request.OrderId); // ✅ استخدام الاستثناء المخصص

            var existingPayment = await _paymentRepository.GetByOrderIdAsync(request.OrderId);
            if (existingPayment != null)
                throw new ConflictException("يوجد دفع مسجل بالفعل لهذا الطلب");

            if (request.PaymentMethod != PaymentMethod.Cash && string.IsNullOrWhiteSpace(request.TransactionReference))
                throw new ValidationException("رقم المرجع مطلوب للدفع الإلكتروني");

            var payment = _mapper.Map<Payment>(request);
            payment.CreatedAt = DateTime.UtcNow;
            payment.UpdatedAt = DateTime.UtcNow;
            payment.Status = PaymentStatus.Pending;

            await _paymentRepository.AddAsync(payment);
            return _mapper.Map<PaymentResponseDto>(payment);
        }

        public async Task<PaymentResponseDto> UpdatePaymentStatusAsync(Guid id, UpdatePaymentStatusRequestDto request)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment == null)
                throw new NotFoundException("Payment", id);

            if (payment.Status is PaymentStatus.Completed or PaymentStatus.Refunded)
                throw new ConflictException("لا يمكن تعديل حالة دفع منتهي");

            payment.Status = request.NewStatus;
            payment.UpdatedAt = DateTime.UtcNow;

            if (request.NewStatus == PaymentStatus.Completed)
            {
                payment.PaymentDate = DateTime.UtcNow;

                // 🚀 تحديث حالة الطلب والطاولة
                var order = await _orderRepository.GetByIdAsync(payment.OrderId);
                if (order != null)
                {
                    order.Status = OrderStatus.Completed;
                    await _orderRepository.UpdateAsync(order);

                    if (order.TableId.HasValue)
                    {
                        var table = await _tableRepository.GetByIdAsync(order.TableId.Value);
                        if (table != null)
                        {
                            table.Status = TableStatus.Available;
                            await _tableRepository.UpdateAsync(table);
                        }
                    }
                }
            }

            await _paymentRepository.UpdateAsync(payment);
            return _mapper.Map<PaymentResponseDto>(payment);
        }

        // باقي الميثودات (Get, Refund) تبقى كما هي مع تبديل Exception بـ NotFoundException
        public async Task<PaymentResponseDto> GetPaymentByIdAsync(Guid id)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment == null) throw new NotFoundException("Payment", id);
            return _mapper.Map<PaymentResponseDto>(payment);
        }

        public async Task<bool> RefundPaymentAsync(Guid id, string? notes)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment == null) throw new NotFoundException("Payment", id);

            if (payment.Status != PaymentStatus.Completed)
                throw new ConflictException("لا يمكن استرداد دفع غير مكتمل");

            payment.Status = PaymentStatus.Refunded;
            payment.Notes = notes ?? payment.Notes;
            payment.UpdatedAt = DateTime.UtcNow;

            await _paymentRepository.UpdateAsync(payment);
            return true;
        }

        public async Task<bool> IsOrderPaidAsync(Guid orderId)
            => await _paymentRepository.IsOrderPaidAsync(orderId);

        public async Task<PaymentResponseDto> GetPaymentByOrderIdAsync(Guid orderId)
        {
            var payment = await _paymentRepository.GetByOrderIdAsync(orderId);
            if (payment == null) throw new NotFoundException("Payment for Order", orderId);
            return _mapper.Map<PaymentResponseDto>(payment);
        }

        public async Task<IEnumerable<PaymentResponseDto>> GetAllPaymentsAsync()
        {
            var payments = await _paymentRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<PaymentResponseDto>>(payments);
        }

        public async Task<IEnumerable<PaymentResponseDto>> GetPaymentsByStatusAsync(PaymentStatus status)
        {
            var payments = await _paymentRepository.GetByStatusAsync(status);
            return _mapper.Map<IEnumerable<PaymentResponseDto>>(payments);
        }
    }
}