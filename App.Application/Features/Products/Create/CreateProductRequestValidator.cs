using App.Application.Contracts.Persistance;
using FluentValidation;
namespace App.Application.Features.Products.Create;

public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    private readonly IProductRepository _productRepository;
    public CreateProductRequestValidator(IProductRepository productRepository)
    {
        _productRepository = productRepository;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Ürün üsmi gereklidir.")
            .Length(3, 10).WithMessage("Ürün ismi 3 ile 10 karakter arasında olmalıdır.");
        //.MustAsync(MustUniqueProductNameAsync).WithMessage("Ürün ismi veritabanında bulunmaktadır.");
        //.Must(MustUniqueProductName).WithMessage("Ürün ismi veritabanında bulunmaktadır.");


        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Ürün fiyatı 0'dan büyük olmalıdır.");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Ürün kategori değeri 0'dan büyük olmalıdır.");

        RuleFor(x => x.Stock)
            .InclusiveBetween(1, 100).WithMessage("Stok adedi 1 ile 100 arasında olmalıdır.");


    }

    #region 2.way async validation
    //private async Task<bool> MustUniqueProductNameAsync(string name,CancellationToken cancellationToken)
    //{
    //    return !await _productRepository.Where(x => x.Name == name).AnyAsync(cancellationToken);
    //}
    #endregion

    #region 1.way sync validation
    //private bool MustUniqueProductName(string name)
    //{
    //    return !_productRepository.Where(x => x.Name == name).Any();
    //}
    #endregion
}
