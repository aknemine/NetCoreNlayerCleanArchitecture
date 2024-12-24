using App.Application.Features.Products.Create;
using App.Application.Features.Products.Update;
using App.Application.Features.UpdateStock;

namespace App.Application.Features.Products
{
    public interface IProductService
    {
        Task<ApplicationResult<List<ProductDto>>> GetTopPriceProductsAsync(int count);
        Task<ApplicationResult<List<ProductDto>>> GetAllListAsync();
        Task<ApplicationResult<List<ProductDto>>> GetPagedAllListAsync(int pageNumber, int pageSize);
        Task<ApplicationResult<ProductDto?>> GetByIdAsync(int id);
        Task<ApplicationResult<CreateProductResponse>> CreateAsync(CreateProductRequest request);
        Task<ApplicationResult> UpdateAsync(int id, UpdateProductRequest request);
        Task<ApplicationResult> UpdateStock(UpdateProductStockRequest request);
        Task<ApplicationResult> DeleteAsync(int id);
    }
}
