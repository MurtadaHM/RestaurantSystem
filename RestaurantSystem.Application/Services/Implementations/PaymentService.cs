using AutoMapper;
using RestaurantSystem.Application.Contracts.Repositories;
using RestaurantSystem.Application.DTOs.Payments;
using RestaurantSystem.Application.Services.Interfaces;
using RestaurantSystem.Domain.Entities;
using RestaurantSystem.Domain.Enums;

namespace RestaurantSystem.Application.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IOrderRepository orderRepository,
            IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        // ──────────────────────────────────────────
        // Create
        // ──────────────────────────────────────────
        public async Task<PaymentResponseDto> CreatePaymentAsync(
            CreatePaymentRequestDto request)
        {
            // ✅ نتحقق من وجود الطلب
            var order = await _orderRepository.GetByIdAsync(request.OrderId);
            if (order == null)
                throw new Exception("الطلب غير موجود");

            // ✅ نمنع تكرار الدفع لنفس الطلب
            var existingPayment = await _paymentRepository.GetByOrderIdAsync(request.OrderId);
            if (existingPayment != null)
                throw new Exception("يوجد دفع مسجل بالفعل لهذا الطلب");

            // ✅ الدفع الإلكتروني يحتاج TransactionReference
            if (request.PaymentMethod != PaymentMethod.Cash &&
                string.IsNullOrWhiteSpace(request.TransactionReference))
                throw new Exception("رقم المرجع مطلوب للدفع الإلكتروني");

            var payment = _mapper.Map<Payment>(request);
            payment.CreatedAt = DateTime.UtcNow;
            payment.UpdatedAt = DateTime.UtcNow;
            payment.Status = PaymentStatus.Pending;

            await _paymentRepository.AddAsync(payment);

            return _mapper.Map<PaymentResponseDto>(payment);
        }

        // ──────────────────────────────────────────
        // Read
        // ──────────────────────────────────────────
        public async Task<PaymentResponseDto> GetPaymentByIdAsync(Guid id)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment == null)
                throw new Exception("الدفع غير موجود");

            return _mapper.Map<PaymentResponseDto>(payment);
        }

        public async Task<PaymentResponseDto> GetPaymentByOrderIdAsync(Guid orderId)
        {
            var payment = await _paymentRepository.GetByOrderIdAsync(orderId);
            if (payment == null)
                throw new Exception("لا يوجد دفع لهذا الطلب");

            return _mapper.Map<PaymentResponseDto>(payment);
        }

        public async Task<IEnumerable<PaymentResponseDto>> GetAllPaymentsAsync()
        {
            var payments = await _paymentRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<PaymentResponseDto>>(payments);
        }

        public async Task<IEnumerable<PaymentResponseDto>> GetPaymentsByStatusAsync(
            PaymentStatus status)
        {
            var payments = await _paymentRepository.GetByStatusAsync(status);
            return _mapper.Map<IEnumerable<PaymentResponseDto>>(payments);
        }

        // ──────────────────────────────────────────
        // Update
        // ──────────────────────────────────────────
        public async Task<PaymentResponseDto> UpdatePaymentStatusAsync(
            Guid id, UpdatePaymentStatusRequestDto request)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment == null)
                throw new Exception("الدفع غير موجود");

            // ✅ نمنع تغيير حالة الدفع المكتمل أو المسترد
            if (payment.Status is PaymentStatus.Completed or PaymentStatus.Refunded)
                throw new Exception("لا يمكن تعديل حالة دفع مكتمل أو مسترد");

            payment.Status = request.NewStatus;
            payment.Notes = request.Notes ?? payment.Notes;
            payment.UpdatedAt = DateTime.UtcNow;

            // ✅ نسجل رقم المرجع عند الإكمال
            if (request.NewStatus == PaymentStatus.Completed)
            {
                payment.PaymentDate = DateTime.UtcNow;
                payment.TransactionReference = request.TransactionReference
                                                ?? payment.TransactionReference;
            }

            await _paymentRepository.UpdateAsync(payment);
            return _mapper.Map<PaymentResponseDto>(payment);
        }

        // ──────────────────────────────────────────
        // Business Logic
        // ──────────────────────────────────────────
        public async Task<bool> RefundPaymentAsync(Guid id, string? notes)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment == null)
                return false;

            // ✅ فقط المدفوعات المكتملة يمكن استردادها
            if (payment.Status != PaymentStatus.Completed)
                throw new Exception("لا يمكن استرداد دفع غير مكتمل");

            payment.Status = PaymentStatus.Refunded;
            payment.Notes = notes ?? payment.Notes;
            payment.UpdatedAt = DateTime.UtcNow;

            await _paymentRepository.UpdateAsync(payment);
            return true;
        }

        public async Task<bool> IsOrderPaidAsync(Guid orderId)
            => await _paymentRepository.IsOrderPaidAsync(orderId);
    }
}
