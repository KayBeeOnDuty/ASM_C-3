using ASM_C_3.Models;
using System.Threading.Tasks;

namespace ASM_C_3.Interface
{
    public interface ICartService
    {

        Task<Cart> GetOrCreateCartAsync(int userId);

        Task<Cart?> GetCartWithItemsAsync(int userId);

        Task AddItemAsync(int userId, int variantId, int quantity);
        Task RemoveItemAsync(int cartDetailId);
        Task ClearCartAsync(int userId);
        Task<Invoice> CheckoutAsync(int userId, PayMethod? payMethod);
    }
}
