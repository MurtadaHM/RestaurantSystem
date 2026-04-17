using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace RestaurantSystem.Api.Hubs
{
    /// <summary>
    /// Hub المسؤول عن التواصل اللحظي لعمليات الطلبات والتوصيل
    /// </summary>
    public class OrderHub : Hub
    {
        // 1. انضمام الموظفين (شيف، مدير، كاشير) لغرفة الإدارة
        // لكي يستلموا تنبيهات بوجود طلبات جديدة فور إنشائها
        public async Task JoinStaffGroup()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "StaffGroup");
        }

        // 2. انضمام الزبون لغرفة خاصة بطلبه فقط (Order Tracking Group)
        // لكي لا تصله إشعارات طلبات الآخرين، بل تحديثات طلبه هو فقط
        public async Task JoinOrderTrackingGroup(string orderId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Order_{orderId}");
        }

        // 3. انضمام الشيف لقسم معين (مثل قسم المشويات فقط)
        public async Task JoinDepartmentGroup(string departmentId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Dept_{departmentId}");
        }

        // 4. ميثود إرسال تنبيه عام (للطوارئ أو الإعلانات داخل المطعم)
        public async Task SendBroadcastNotification(string title, string message)
        {
            await Clients.All.SendAsync("ReceiveBroadcast", new { title, message });
        }

       
    }
}