using App.Domain.Entities;

namespace App.Application.Contracts.Persistance;

public interface IProductRepository : IGenericRepository<Product, int>
{
    public Task<List<Product>> GetTopPriceProductsAsync(int count);
}

