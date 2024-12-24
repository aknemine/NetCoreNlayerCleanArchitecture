using App.Application.Contracts.Persistance;
using App.Application.Features.Products.Create;
using App.Application.Features.Products.Update;
using App.Application.Features.UpdateStock;
using App.Domain.Entities;
using AutoMapper;
using FluentValidation;
using System.Net;

namespace App.Application.Features.Products;

public class ProductService(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    IValidator<CreateProductRequest> createProductRequestValidator,
    IMapper mapper) : IProductService
{
    public async Task<ApplicationResult<List<ProductDto>>> GetTopPriceProductsAsync(int count)
    {
        var products= await productRepository.GetTopPriceProductsAsync(count);

        //var productList = products.Select(i => new ProductDto
        //    (i.Id,i.Name,i.Price,i.Stock)).ToList();//manuel mapping işlemi daha hızlı yazılır

        var productsAsDto = mapper.Map<List<ProductDto>>(products);

        return new ApplicationResult<List<ProductDto>>() { 
            Data=productsAsDto
        };
    }

    public async Task<ApplicationResult<List<ProductDto>>> GetAllListAsync()
    {
        var products = await productRepository.GetAllAsync();

        #region manuel mapping
        //var productAsDto = products.Select(i => new ProductDto(i.Id,i.Name,i.Price,i.Stock)).ToList();
        #endregion

        var productAsDto =mapper.Map<List<ProductDto>>(products);  

        return ApplicationResult<List<ProductDto>>.Success(productAsDto);
    }

    public async Task<ApplicationResult<List<ProductDto>>> GetPagedAllListAsync(int pageNumber,int pageSize)
    {
        //1-10=>ilk 10 kayıt skip(0).Take(10)
        //2-10=>11-20 kayıt skip(10).Take(10)
        //3-10=>21-30 kayıt skip(10).Take(10)

        
        var products=await productRepository.GetAllPagedAsync(pageNumber,pageSize);

        #region manuel mapping
        //var productAsDto = products.Select(i => new ProductDto(i.Id, i.Name, i.Price, i.Stock)).ToList();
        #endregion

        var productAsDto = mapper.Map<List<ProductDto>>(products);

        return ApplicationResult<List<ProductDto>>.Success(productAsDto);
    }

    public async Task<ApplicationResult<ProductDto?>> GetByIdAsync(int id)
    {
        var product = await productRepository.GetByIdAsync(id);

        if (product is null)
        {
            return ApplicationResult<ProductDto?>.Fail("Product not found", HttpStatusCode.NotFound);
        }

        #region manuel mapping
        //var productAsDto = new ProductDto(product!.Id, product.Name, product.Price, product.Stock);
        #endregion

        var productAsDto = mapper.Map<ProductDto>(product);

        return ApplicationResult<ProductDto>.Success(productAsDto)!;
    }

    public async Task<ApplicationResult<CreateProductResponse>> CreateAsync(CreateProductRequest request)
    {
        //throw new CriticalException("kritik seviyede bir hata meydana geldi");
        //throw new Exception("db hatası");

        // async manuel service busines check
        var anyProduct = await productRepository.AnyAsync(x => x.Name == request.Name);

        if (anyProduct)
        {
            return ApplicationResult<CreateProductResponse>.Fail("Ürün ismi veritabanında bulunmaktadır.", HttpStatusCode.BadRequest);
        }


        #region async manuel fluent validation busines check
        //var validationResult = await createProductRequestValidator.ValidateAsync(request);
        //var validationResult = await createProductRequestValidator.ValidateAsync(request);

        //if (!validationResult.IsValid)
        //{
        //    return ServiceResult<CreateProductResponse>.Fail(
        //        validationResult.Errors.Select(x => x.ErrorMessage).ToList());
        //}
        #endregion

        //manuel mapping
        //var product = new Product
        //{
        //    Name = request.Name,
        //    Price = request.Price,
        //    Stock = request.Stock,
        //};

        var product=mapper.Map<Product>(request);

        await productRepository.AddAsync(product);
        await unitOfWork.SaveChangesAsync();

        return ApplicationResult<CreateProductResponse>.SuccessAsCreated(new CreateProductResponse(product.Id),$"api/products/{product.Id}");
    }

    public async Task<ApplicationResult> UpdateAsync(int id, UpdateProductRequest request)
    {
        ///Clean Code
        ///Fast fail:önce olumsuz durumları dön
        ///Guard Clauses:mümkün olduğunca sadece if yaz else yazma

        
        var isProductNameExist = await productRepository.AnyAsync(x => x.Name == request.Name && x.Id != id);

        if (isProductNameExist)
        {
            return ApplicationResult.Fail("Ürün ismi veritabanında bulunmaktadır.", HttpStatusCode.BadRequest);
        }

        //manuel mapping
        //product.Name=request.Name;
        //product.Price=request.Price;
        //product.Stock=request.Stock;

        var product=mapper.Map<Product>(request);
        product.Id = id;

        productRepository.Update(product);
        await unitOfWork.SaveChangesAsync();

        return ApplicationResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ApplicationResult> UpdateStock(UpdateProductStockRequest request)
    {
        var product = await productRepository.GetByIdAsync(request.productId);

        if (product is null)
        {
            return ApplicationResult.Fail("Ürün bulunamadı.",HttpStatusCode.NotFound);
        }

        product.Stock = request.Quantity;

        productRepository.Update(product);
        await unitOfWork.SaveChangesAsync();

        return ApplicationResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ApplicationResult> DeleteAsync(int id)
    {
        var product = await productRepository.GetByIdAsync(id);

        productRepository.Delete(product);
        await unitOfWork.SaveChangesAsync();

        return ApplicationResult.Success(HttpStatusCode.NoContent);
    }
}
