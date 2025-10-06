using ASM_C_3.Models;

namespace ASM_C_3.Interface
{
    public interface IVariantService
    {
        Task<List<Variant>> GetAllAsync();
        Task<Variant> GetByIdAsync(int id);
        Task AddAsync(Variant variant);
        Task UpdateAsync(Variant variant);
        Task DeleteAsync(int id);
    }
}
