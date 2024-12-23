﻿using App.Repositories.Products;
using App.Services.Products.Create;
using App.Services.Products.Update;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.TagHelpers;

namespace App.Services.Products;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductDto>().ReverseMap();
        CreateMap<CreateProductRequest, Product>().ForMember(dest => dest.Name,
            opt => opt.MapFrom(src => src.Name.ToLowerInvariant()
            ));
        CreateMap<UpdateProductRequest, Product>().ForMember(dest => dest.Name,
            opt => opt.MapFrom(src => src.Name.ToLowerInvariant()
            ));
    }
}