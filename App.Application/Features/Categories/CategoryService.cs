using App.Application.Contracts.Persistance;
using App.Application.Features.Categories.Create;
using App.Application.Features.Categories.Dto;
using App.Application.Features.Categories.Update;
using App.Domain.Entities;
using AutoMapper;
using System.Net;

namespace App.Application.Features.Categories;

public class CategoryService(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, IMapper mapper) : ICategoryService
{
    public async Task<ApplicationResult<CategoryWithProductsDto>> GetCategoryWithProductsAsync(int categoryId)
    {
        var category=await categoryRepository.GetCategoryWithProductsAsync(categoryId);

        if (category is null)
        {
            return ApplicationResult<CategoryWithProductsDto>.Fail("Kategori bulunamadı.", HttpStatusCode.NotFound);
        }

        var categoryAsDto=mapper.Map<CategoryWithProductsDto>(category);

        return ApplicationResult<CategoryWithProductsDto>.Success(categoryAsDto);
    }

    public async Task<ApplicationResult<List<CategoryWithProductsDto>>> GetCategoryWithProductsAsync()
    {
        var categories = await categoryRepository.GetCategoryWithProductsAsync();

        var categoriesAsDto = mapper.Map<List<CategoryWithProductsDto>>(categories);

        return ApplicationResult<List<CategoryWithProductsDto>>.Success(categoriesAsDto);
    }

    public async Task<ApplicationResult<List<CategoryDto>>> GetAllListAsync()
    {
        var categories = await categoryRepository.GetAllAsync();

        var categoriesAsDto=mapper.Map<List<CategoryDto>>(categories);

        return ApplicationResult<List<CategoryDto>>.Success(categoriesAsDto);
    }

    public async Task<ApplicationResult<CategoryDto>> GetByIdAsync(int id)
    {
        var category = await categoryRepository.GetByIdAsync(id);

        if (category is null)
        {
            return ApplicationResult<CategoryDto>.Fail("Kategori bulunamadı.",HttpStatusCode.NotFound);
        }

        var categoryAsDto = mapper.Map<CategoryDto>(category);

        return ApplicationResult<CategoryDto>.Success(categoryAsDto);
    }

    public  async Task<ApplicationResult<int>> CreateAsync(CreateCategoryRequest request)
    {
        var anyCategory = await categoryRepository.AnyAsync(x => x.Name == request.Name);

        if (anyCategory)
        {
            return ApplicationResult<int>.Fail("Categori ismi veritabanında bulunmaktadır.", HttpStatusCode.NotFound);
        }

        var newCategory=mapper.Map<Category>(request);

        await categoryRepository.AddAsync(newCategory);
        await unitOfWork.SaveChangesAsync();

        return ApplicationResult<int>.SuccessAsCreated(newCategory.Id, $"api/categories/{newCategory.Id}");
    }

    public async Task<ApplicationResult> UpdateAsync(int id,UpdateCategoryRequest request)
    {
        
        var isCategorNameExist = await categoryRepository.AnyAsync(x => x.Name == request.Name && x.Id != id);

        if (isCategorNameExist)
        {
            return ApplicationResult.Fail("Kategori ismi veritabanında bulunmaktadır.", HttpStatusCode.BadRequest);
        }

        var category=mapper.Map<Category>(request);
        category.Id = id;

        categoryRepository.Update(category);
        await unitOfWork.SaveChangesAsync();

        return ApplicationResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ApplicationResult> DeleteAsync(int id)
    {
        var category = await categoryRepository.GetByIdAsync(id);

        categoryRepository.Delete(category);
        await unitOfWork.SaveChangesAsync();

        return ApplicationResult.Success(HttpStatusCode.NoContent);
    }

}
