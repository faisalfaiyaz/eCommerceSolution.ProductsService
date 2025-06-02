using AutoMapper;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.ServiceContracts;
using DataAccessLayer.Entities;
using DataAccessLayer.RepositoryContracts;
using FluentValidation;
using FluentValidation.Results;
using System.Linq.Expressions;

namespace BusinessLogicLayer.Services;

public class ProductService : IProductService
{

    private readonly IProductRepository _productRepository;
    private readonly IValidator<ProductAddRequest> _productAddRequestValidator;
    private readonly IValidator<ProductUpdateRequest> _productUpdateRequestValidator;
    private readonly IMapper _mapper;

    public ProductService(IProductRepository productRepository, 
                          IValidator<ProductAddRequest> productAddRequestValidator,
                          IValidator<ProductUpdateRequest> productUpdateRequestValidator,
                          IMapper mapper)
    {
        _productRepository = productRepository;
        _productAddRequestValidator = productAddRequestValidator;
        _productUpdateRequestValidator = productUpdateRequestValidator;
        _mapper = mapper;
    }


    public async Task<ProductResponse?> AddProduct(ProductAddRequest productAddRequest)
    {
        if (productAddRequest == null)
        {
            throw new ArgumentNullException(nameof(productAddRequest));
        }

        ValidationResult validationResult = await _productAddRequestValidator.ValidateAsync(productAddRequest);

        if (!validationResult.IsValid)
        {
            IEnumerable<string> listOfErrors = validationResult.Errors.Select(vf => vf.ErrorMessage);

            throw new ArgumentNullException(string.Join(", ", listOfErrors));
        }

        Product? addedProduct = await _productRepository.AddProduct(_mapper.Map<Product>(productAddRequest));

        if (addedProduct == null)
        {
            return null;
        }

        return _mapper.Map<ProductResponse>(addedProduct);

    }

    public async Task<bool> DeleteProduct(Guid productID)
    {
        Product? product = await _productRepository.GetProductByCondtion(p => p.ProductID == productID);

        if (product == null)
        {
            return false;
        }

        return await _productRepository.DeleteProduct(productID);
    }

    public async Task<ProductResponse?> GetProductByCondition(Expression<Func<Product, bool>> conditionExpression)
    {
        Product? product = await _productRepository.GetProductByCondtion(conditionExpression);
        if (product is null)
        {
            return null;
        }
        return _mapper.Map<ProductResponse>(product);
    }

    public async Task<List<ProductResponse?>> GetProducts()
    {
        IEnumerable<Product> products = await _productRepository.GetProducts();
        return _mapper.Map<List<ProductResponse>>(products)!;
    }

    public async Task<List<ProductResponse?>> GetProductsByCondition(Expression<Func<Product, bool>> conditionExpression)
    {
        IEnumerable<Product?> filteredProducts = await _productRepository.GetProductsByCondtion(conditionExpression);
        if (filteredProducts == null)
        {
            return null;
        }

        return _mapper.Map<List<ProductResponse?>>(filteredProducts);
    }

    public async Task<ProductResponse?> UpdateProduct(ProductUpdateRequest productUpdateRequest)
    {

        Product? existinProduct = await _productRepository.GetProductByCondtion(p => p.ProductID == productUpdateRequest.ProductID);


        if (existinProduct is null)
        {
            throw new ArgumentNullException("Invalid ProductId");
        }

        ValidationResult validationResult = await _productUpdateRequestValidator.ValidateAsync(productUpdateRequest);
        if (!validationResult.IsValid)
        {
            IEnumerable<string> listOfErrors = validationResult.Errors.Select(vf => vf.ErrorMessage);

            throw new ArgumentNullException(string.Join(", ", listOfErrors));
        }

        Product product = _mapper.Map<Product>(productUpdateRequest);
        Product? updatedProduct = await _productRepository.UpdateProduct(product);

        return _mapper.Map<ProductResponse>(updatedProduct);
    }
}
