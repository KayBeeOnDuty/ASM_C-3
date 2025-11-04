using ASM_C_3.Data;
using ASM_C_3.Interface;
using ASM_C_3.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ASM_C_3.Service
{
    public class CartService : ICartService
    {
        private readonly TraNgheDbContext _context;

        public CartService(TraNgheDbContext context)
        {
            _context = context;
        }

        public async Task<Cart> GetOrCreateCartAsync(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartDetails)
                .ThenInclude(cd => cd.Variant)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CreatedDate = DateTime.Now
                };

                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }

        public async Task<Cart?> GetCartWithItemsAsync(int userId)
        {
            return await _context.Carts
                .Include(c => c.CartDetails)
                    .ThenInclude(cd => cd.Variant)
                        .ThenInclude(v => v.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task AddItemAsync(int userId, int variantId, int quantity)
        {
            if (quantity <= 0) quantity = 1;

            var variant = await _context.Variants.FirstOrDefaultAsync(v => v.VariantId == variantId)
                          ?? throw new InvalidOperationException("Sản phẩm không tồn tại.");

            var cart = await GetOrCreateCartAsync(userId);

            var existingDetail = await _context.CartDetails
                .FirstOrDefaultAsync(cd => cd.CartId == cart.CartId && cd.VariantId == variantId);

            if (existingDetail != null)
            {
                existingDetail.Quantity += quantity;
                existingDetail.UnitPrice = (int)variant.Price; // ép kiểu decimal → int
                existingDetail.SubTotal = existingDetail.Quantity * existingDetail.UnitPrice;
            }
            else
            {
                var newDetail = new CartDetail
                {
                    CartId = cart.CartId,
                    VariantId = variantId,
                    Quantity = quantity,
                    UnitPrice = (int)variant.Price,
                    SubTotal = (int)(variant.Price * quantity)
                };
                _context.CartDetails.Add(newDetail);
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveItemAsync(int cartDetailId)
        {
            var detail = await _context.CartDetails.FindAsync(cartDetailId);
            if (detail != null)
            {
                _context.CartDetails.Remove(detail);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartDetails)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart != null)
            {
                _context.CartDetails.RemoveRange(cart.CartDetails);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Invoice> CheckoutAsync(int userId, PayMethod? payMethod)
        {
            var cart = await GetCartWithItemsAsync(userId)
                       ?? throw new InvalidOperationException("Không tìm thấy giỏ hàng.");

            if (!cart.CartDetails.Any())
                throw new InvalidOperationException("Giỏ hàng của bạn đang trống.");

            decimal totalPrice = cart.CartDetails.Sum(cd => cd.SubTotal);

            var invoice = new Invoice
            {
                UserId = userId,
                PayMethod = payMethod,
                TotalPrice = (int)totalPrice,
                CreatedDate = DateTime.Now,
                Status = OrderStatus.Pending,
                IsDeleted = false,
                TotalAmount = (int)totalPrice
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            foreach (var item in cart.CartDetails)
            {
                var invoiceDetail = new InvoiceDetail
                {
                    InvoiceId = invoice.InvoiceId,
                    VariantId = item.VariantId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    SubTotal = item.SubTotal
                };
                _context.InvoiceDetails.Add(invoiceDetail);
            }

            _context.CartDetails.RemoveRange(cart.CartDetails);
            await _context.SaveChangesAsync();

            return invoice;
        }
    }
}
