using App.Application.Contracts.Persistance;
using App.Domain.Entities;

namespace App.Application.Contracts.Persistance;

public interface ICategoryRepository:IGenericRepository<Category,int>
{
    Task<Category?> GetCategoryWithProductsAsync(int id);
    Task<List<Category>> GetCategoryWithProductsAsync();
}
