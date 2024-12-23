﻿using App.Repositories;
using App.Repositories.Categories;
using App.Services.Categories.Create;
using App.Services.Categories.Dto;
using App.Services.Categories.Update;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace App.Services.Categories;

public class CategoryService(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, IMapper mapper) : ICategoryService
{
    public async Task<ServiceResult<CategoryWithProductsDto>> GetCategoryWithProductsAsync(int categoryId)
    {
        var category=await categoryRepository.GetCategoryWithProductsAsync(categoryId);

        if (category is null)
        {
            return ServiceResult<CategoryWithProductsDto>.Fail("Kategori bulunamadı.", HttpStatusCode.NotFound);
        }

        var categoryAsDto=mapper.Map<CategoryWithProductsDto>(category);

        return ServiceResult<CategoryWithProductsDto>.Success(categoryAsDto);
    }

    public async Task<ServiceResult<List<CategoryWithProductsDto>>> GetCategoryWithProductsAsync()
    {
        var categories = await categoryRepository.GetCategoryWithProducts().ToListAsync();

        var categoriesAsDto = mapper.Map<List<CategoryWithProductsDto>>(categories);

        return ServiceResult<List<CategoryWithProductsDto>>.Success(categoriesAsDto);
    }

    public async Task<ServiceResult<List<CategoryDto>>> GetAllListAsync()
    {
        var categories = await categoryRepository.GetAll().ToListAsync();

        var categoriesAsDto=mapper.Map<List<CategoryDto>>(categories);

        return ServiceResult<List<CategoryDto>>.Success(categoriesAsDto);
    }

    public async Task<ServiceResult<CategoryDto>> GetByIdAsync(int id)
    {
        var category = await categoryRepository.GetByIdAsync(id);

        if (category is null)
        {
            return ServiceResult<CategoryDto>.Fail("Kategori bulunamadı.",HttpStatusCode.NotFound);
        }

        var categoryAsDto = mapper.Map<CategoryDto>(category);

        return ServiceResult<CategoryDto>.Success(categoryAsDto);
    }

    public  async Task<ServiceResult<int>> CreateAsync(CreateCategoryRequest request)
    {
        var anyCategory = await categoryRepository.Where(x => x.Name == request.Name).AnyAsync();

        if (anyCategory)
        {
            return ServiceResult<int>.Fail("Categori ismi veritabanında bulunmaktadır.",HttpStatusCode.NotFound);
        }

        var newCategory=mapper.Map<Category>(request);

        await categoryRepository.AddAsync(newCategory);
        await unitOfWork.SaveChangesAsync();

        return ServiceResult<int>.SuccessAsCreated(newCategory.Id,$"api/categories/{newCategory.Id}");
    }

    public async Task<ServiceResult> UpdateAsync(int id,UpdateCategoryRequest request)
    {
        
        var isCategorNameExist = await categoryRepository.Where(x => x.Name == request.Name && x.Id != id).AnyAsync();

        if (isCategorNameExist)
        {
            return ServiceResult.Fail("Kategori ismi veritabanında bulunmaktadır.", HttpStatusCode.BadRequest);
        }

        var category=mapper.Map<Category>(request);
        category.Id = id;

        categoryRepository.Update(category);
        await unitOfWork.SaveChangesAsync();

        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        var category = await categoryRepository.GetByIdAsync(id);

        categoryRepository.Delete(category);
        await unitOfWork.SaveChangesAsync();

        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

}
