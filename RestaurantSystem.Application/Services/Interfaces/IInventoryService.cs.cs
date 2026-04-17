using RestaurantSystem.Application.DTOs.Inventory;

namespace RestaurantSystem.Application.Services.Interfaces
{
    public interface IInventoryService
    {
        // ==========================================
        // 1. العمليات الأساسية (Management)
        // ==========================================

        // جلب كل المواد الأولية (للعرض في جدول المخزن)
        Task<IEnumerable<IngredientResponseDto>> GetAllIngredientsAsync();

        // جلب مادة معينة بالتفصيل
        Task<IngredientResponseDto> GetIngredientByIdAsync(Guid id);

        // إنشاء مادة جديدة لأول مرة (لحم، بصل، الخ...)
        Task<IngredientResponseDto> CreateIngredientAsync(CreateIngredientRequestDto request);

        // 🔄 تحديث بيانات مادة موجودة (الاسم، السعر، حد الإنذار)
        Task<bool> UpdateIngredientAsync(Guid id, CreateIngredientRequestDto request);


        // ==========================================
        // 2. المحرك الذكي (The Intelligence)
        // ==========================================

        // الدالة الأهم: الخصم التلقائي للمخزن فور إتمام الطلب
        Task<bool> ProcessOrderStockDeductionAsync(Guid orderId);

        // إضافة مشتريات للمخزن (زيادة الكمية)
        Task<bool> AddStockAsync(AddStockRequestDto request);

        // جلب المواد التي وصلت لخط الخطر (Low Stock) للتنبيهات
        Task<IEnumerable<IngredientResponseDto>> GetLowStockIngredientsAsync();


        // ==========================================
        // 3. إدارة الوصفات (Recipes)
        // ==========================================

        // ربط صنف من المنيو بمكوناته برمجياً
        Task<bool> UpdateRecipeAsync(Guid menuItemId, List<MenuItemIngredientDto> ingredients);

        // جلب مكونات طبق معين
        Task<IEnumerable<MenuItemIngredientDto>> GetRecipeByMenuItemIdAsync(Guid menuItemId);


        // ==========================================
        // 4. التقارير والتدقيق (Auditing)
        // ==========================================

        // جلب تاريخ حركة مادة معينة (مبيعات، مشتريات، تلف) لزر السجل 📜
        Task<IEnumerable<StockMovementResponseDto>> GetStockHistoryAsync(Guid ingredientId);
    }
}