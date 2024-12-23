using App.Repositories;
using App.Repositories.Products;
using App.Services.ExceptionHandlers;
using App.Services.Products.Create;
using App.Services.Products.Update;
using App.Services.Products.UpdateStock;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Products
{
    public class ProductService(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        IValidator<CreateProductRequest> createProductRequestValidator,
        IMapper mapper) :IProductService
    {
        public async Task<ServiceResult<List<ProductDto>>> GetTopPriceProductsAsync(int count)
        {
            var products= await productRepository.GetTopPriceProductsAsync(count);

            //var productList = products.Select(i => new ProductDto
            //    (i.Id,i.Name,i.Price,i.Stock)).ToList();//manuel mapping işlemi daha hızlı yazılır

            var productsAsDto = mapper.Map<List<ProductDto>>(products);

            return new ServiceResult<List<ProductDto>>() { 
                Data=productsAsDto
            };
        }

        public async Task<ServiceResult<List<ProductDto>>> GetAllListAsync()
        {
            var products = await productRepository.GetAll().ToListAsync();

            #region manuel mapping
            //var productAsDto = products.Select(i => new ProductDto(i.Id,i.Name,i.Price,i.Stock)).ToList();
            #endregion

            var productAsDto =mapper.Map<List<ProductDto>>(products);  

            return ServiceResult<List<ProductDto>>.Success(productAsDto);
        }

        public async Task<ServiceResult<List<ProductDto>>> GetPagedAllListAsync(int pageNumber,int pageSize)
        {
            //1-10=>ilk 10 kayıt skip(0).Take(10)
            //2-10=>11-20 kayıt skip(10).Take(10)
            //3-10=>21-30 kayıt skip(10).Take(10)

            int skip = (pageNumber - 1) * pageSize;

            var products=await productRepository.GetAll().Skip(skip).Take(pageSize).ToListAsync();

            #region manuel mapping
            //var productAsDto = products.Select(i => new ProductDto(i.Id, i.Name, i.Price, i.Stock)).ToList();
            #endregion

            var productAsDto = mapper.Map<List<ProductDto>>(products);

            return ServiceResult<List<ProductDto>>.Success(productAsDto);
        }

        public async Task<ServiceResult<ProductDto?>> GetByIdAsync(int id)
        {
            var product = await productRepository.GetByIdAsync(id);

            if (product is null)
            {
                return ServiceResult<ProductDto?>.Fail("Product not found", HttpStatusCode.NotFound);
            }

            #region manuel mapping
            //var productAsDto = new ProductDto(product!.Id, product.Name, product.Price, product.Stock);
            #endregion

            var productAsDto = mapper.Map<ProductDto>(product);

            return ServiceResult<ProductDto>.Success(productAsDto)!;
        }

        public async Task<ServiceResult<CreateProductResponse>> CreateAsync(CreateProductRequest request)
        {
            //throw new CriticalException("kritik seviyede bir hata meydana geldi");
            //throw new Exception("db hatası");

            // async manuel service busines check
            var anyProduct = await productRepository.Where(x => x.Name == request.Name).AnyAsync();

            if (anyProduct)
            {
                return ServiceResult<CreateProductResponse>.Fail("Ürün ismi veritabanında bulunmaktadır.", HttpStatusCode.BadRequest);
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

            return ServiceResult<CreateProductResponse>.SuccessAsCreated(new CreateProductResponse(product.Id),$"api/products/{product.Id}");
        }

        public async Task<ServiceResult> UpdateAsync(int id, UpdateProductRequest request)
        {
            ///Clean Code
            ///Fast fail:önce olumsuz durumları dön
            ///Guard Clauses:mümkün olduğunca sadece if yaz else yazma

            
            var isProductNameExist = await productRepository.Where(x => x.Name == request.Name && x.Id!=id).AnyAsync();

            if (isProductNameExist)
            {
                return ServiceResult.Fail("Ürün ismi veritabanında bulunmaktadır.", HttpStatusCode.BadRequest);
            }

            //manuel mapping
            //product.Name=request.Name;
            //product.Price=request.Price;
            //product.Stock=request.Stock;

            var product=mapper.Map<Product>(request);
            product.Id = id;

            productRepository.Update(product);
            await unitOfWork.SaveChangesAsync();

            return ServiceResult.Success(HttpStatusCode.NoContent);
        }

        public async Task<ServiceResult> UpdateStock(UpdateProductStockRequest request)
        {
            var product = await productRepository.GetByIdAsync(request.productId);

            if (product is null)
            {
                return ServiceResult.Fail("Ürün bulunamadı.",HttpStatusCode.NotFound);
            }

            product.Stock = request.Quantity;

            productRepository.Update(product);
            await unitOfWork.SaveChangesAsync();

            return ServiceResult.Success(HttpStatusCode.NoContent);
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var product = await productRepository.GetByIdAsync(id);

            productRepository.Delete(product);
            await unitOfWork.SaveChangesAsync();

            return ServiceResult.Success(HttpStatusCode.NoContent);
        }
    }
}
