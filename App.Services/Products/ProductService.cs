using App.Repositories;
using App.Repositories.Products;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Products
{
    public class ProductService(IProductRepository productRepository,IUnitOfWork unitOfWork):IProductService
    {
        public async Task<ServiceResult<List<ProductDto>>> GetTopPriceProductsAsync(int count)
        {
            var products= await productRepository.GetTopPriceProductsAsync(count);

            var productList = products.Select(i => new ProductDto
                (i.Id,i.Name,i.Price,i.Stock)).ToList();//manul mapping işlemi daha hızlı yazılır

            return ServiceResult<List<ProductDto>>.Success(productList);
        }

        public async Task<ServiceResult<List<ProductDto>>> GetAllListAsync()
        {
            var products = await productRepository.GetAll().ToListAsync();

            var productAsDto = products.Select(i => new ProductDto(i.Id,i.Name,i.Price,i.Stock)).ToList();

            return ServiceResult<List<ProductDto>>.Success(productAsDto);
        }

        public async Task<ServiceResult<List<ProductDto>>> GetPagedAllListAsync(int pageNumber,int pageSize)
        {
            //1-10=>ilk 10 kayıt skip(0).Take(10)
            //2-10=>11-20 kayıt skip(10).Take(10)
            //3-10=>21-30 kayıt skip(10).Take(10)

            int skip = (pageNumber - 1) * pageSize;

            var products=await productRepository.GetAll().Skip(skip).Take(pageSize).ToListAsync();

            var productAsDto = products.Select(i => new ProductDto(i.Id, i.Name, i.Price, i.Stock)).ToList();

            return ServiceResult<List<ProductDto>>.Success(productAsDto);
        }

        public async Task<ServiceResult<ProductDto?>> GetByIdAsync(int id)
        {
            var product = await productRepository.GetByIdAsync(id);

            if (product is null)
            {
                return ServiceResult<ProductDto?>.Fail("Product not found", HttpStatusCode.NotFound);
            }

            var productAsDto = new ProductDto(product!.Id, product.Name, product.Price, product.Stock);

            return ServiceResult<ProductDto>.Success(productAsDto)!;
        }

        public async Task<ServiceResult<CreateProductResponse>> CreateAsync(CreateProductRequest request)
        {
            var product = new Product
            {
                Name = request.Name,
                Price = request.Price,
                Stock = request.Stock,
            };

            await productRepository.AddAsync(product);
            await unitOfWork.SaveChangesAsync();

            return ServiceResult<CreateProductResponse>.SuccessAsCreated(new CreateProductResponse(product.Id),$"api/products/{product.Id}");
        }

        public async Task<ServiceResult> UpdateAsync(int id, UpdateProductRequest request)
        {
            ///Clean Code
            ///Fast fail:önce olumsuz durumları dön
            ///Guard Clauses:mümkün olduğunca sadece if yaz else yazma

            var product = await productRepository.GetByIdAsync(id);

            if (product is null)
            {
                return ServiceResult.Fail("Product not found", HttpStatusCode.NotFound);
            }

            product.Name=request.Name;
            product.Price=request.Price;
            product.Stock=request.Stock;

            productRepository.Update(product);
            await unitOfWork.SaveChangesAsync();

            return ServiceResult.Success(HttpStatusCode.NoContent);
        }

        public async Task<ServiceResult> UpdateStock(UpdateProductStockRequest request)
        {
            var product = await productRepository.GetByIdAsync(request.productId);

            if (product is null)
            {
                return ServiceResult.Fail("Product not found", HttpStatusCode.NotFound);
            }

            product.Stock = request.Quantity;

            productRepository.Update(product);
            await unitOfWork.SaveChangesAsync();

            return ServiceResult.Success(HttpStatusCode.NoContent);
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            

            var product = await productRepository.GetByIdAsync(id);

            if (product is null)
            {
                return ServiceResult.Fail("Product not found", HttpStatusCode.NotFound);
            }

            productRepository.Delete(product);
            await unitOfWork.SaveChangesAsync();

            return ServiceResult.Success(HttpStatusCode.NoContent);
        }
    }
}
