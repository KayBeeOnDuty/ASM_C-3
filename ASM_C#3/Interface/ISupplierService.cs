using ASM_C_3.Models;

namespace ASM_C_3.Interface
{
    public interface ISupplierService
    {
        Task<IEnumerable<Supplier>> GetAllAsync();

        Task<Supplier?> GetByIdAsync(int id);

        Task AddAsync(Supplier supplier);

        Task UpdateAsync(Supplier supplier);

        Task DeleteAsync(int id);
    }
}
